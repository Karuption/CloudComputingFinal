<template>
  <div class="wrapper">
    <div class="list-wrapper">
      <TheHeader :tabs="tabs" :tasks="tasks" :todo-list="todoList" :completed-list="completedList" @show-todo="showTodo"
        @show-completed="showCompleted" />
      <LoadingSpinner v-if="isLoading" />
      <EmptyStateDisplay v-else :tasks="tasks" />
      <div v-if="todoList">
        <div class="content-wrapper">
          <TodoItem type="todo" :tasks="tasks" :is-checked="isChecked" :full-display="fullDisplay"
            @toggle-completed-task="toggleCompletedTask" @set-checked="setIsChecked" @remove-checked="removeIsChecked"
            @remove-task="removeTask" @toggle-favorite="toggleFavorite" @show-all-item-contents="showAllItemContents" />
        </div>
        <TheFooter :is-logged-in="isLoggedIn" type="buttons" :add-item="addItem" :canvas-login="canvasLogin"
          @toggle-canvas-input="toggleCanvasInput" @toggle-form-input="toggleFormInput" />
      </div>
      <div v-else class="completed-wrapper">
        <TodoItem type="completed" :tasks="tasks" :is-checked="isChecked" :full-display="fullDisplay"
          @toggle-completed-task="toggleCompletedTask" @set-checked="setIsChecked" @remove-checked="removeIsChecked"
          @remove-task="removeTask" @toggle-favorite="toggleFavorite" @show-all-item-contents="showAllItemContents" />
      </div>
    </div>
    <TheFooter type="form" :is-logged-in="isLoggedIn" :add-item="addItem" :canvas-login="canvasLogin"
      @toggle-form-input="toggleFormInput" @submit-canvas-form-details="submitCanvasFormDetails"
      @toggle-canvas-input="toggleCanvasInput" @submit-form="submitForm" @update-title="updateTitle"
      @update-description="updateDescription" @update-due-date="updateDueDate" />
  </div>
</template>

<script>
import axios from 'axios'
import tippy from 'tippy.js'
import 'tippy.js/dist/tippy.css'

import TheHeader from './TheHeader.vue'
import EmptyStateDisplay from './EmptyStateDisplay.vue'
import LoadingSpinner from './LoadingSpinner.vue'
import TodoItem from './TodoItem.vue'
import TheFooter from './TheFooter.vue'
import { useSignalR } from '@dreamonkey/vue-signalr'

export default {
  components: {
    TheHeader,
    EmptyStateDisplay,
    LoadingSpinner,
    TodoItem,
    TheFooter
  },
  props: {
    isLoggedIn: {
      type: Boolean,
      default: false
    }
  },
  setup() {
    const signalr = useSignalR()

    signalr.invoke('Register').catch(err => {
      console.error(err)
    })
  },
  data() {
    return {
      newTask: {
        title: '',
        description: '',
        dueDate: ''
      },
      canvasUrl: '',
      accessToken: '',
      tasks: [],
      isChecked: [],
      fullDisplay: [],
      tabs: true,
      addItem: false,
      todoList: true,
      isLoading: true,
      canvasLogin: false,
      completedList: false
    }
  },
  watch: {
    isLoggedIn: async function (newVal, oldVal) {
      this.loadBackendData()
      const signalr = useSignalR()

      if (oldVal === false && newVal === true) {
        await signalr.invoke('Unregister').catch(err => {
          console.error(err)
        })
      }

      await signalr.invoke('Register').catch(err => {
        console.error(err)
      })
    }
  },
  updated() {
    this.loadTippySettings()
  },
  unmounted() {
    const signalR = useSignalR()
    signalR.off('TaskChanged')
    signalR.invoke('Unregister').catch(err => {
      console.error(err)
    })
  },
  mounted() {
    this.loadBackendData()
    this.loadTippySettings()
    const signalR = useSignalR()
    signalR.on('TaskChanged', () => {
      console.log('TaskChanged')
      this.loadBackendData()
    })
  },
  methods: {
    loadBackendData() {
      console.log('loading data')
      this.isLoading = true
      axios.get(import.meta.env.VITE_API_KEY + '/api/ToDoTask', {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`
        }
      }).then(response => {
        this.tasks = response.data
      })
        .catch(error => {
          console.log(error)
        })
        .finally(() => {
          this.isLoading = false
        })
    },
    submitForm() {
      if (!this.newTask.dueDate) {
        this.newTask.dueDate = null
      }
      this.isLoading = true
      axios.post(import.meta.env.VITE_API_KEY + '/api/ToDoTask', this.newTask, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`
        }
      }).then(response => { })
        .catch(error => {
          console.log(error)
          this.isLoading = false
          this.loadBackendData()
        })
      this.toggleFormInput()
    },
    async submitCanvasFormDetails(formData) {
      if (this.isLoggedIn) {
        try {
          this.isLoading = true
          const response = await axios.post(import.meta.env.VITE_API_KEY + '/api/Authenticate/add-CanvasKey', {
            canvasUrl: formData.canvasUrl,
            accessToken: formData.accessToken
          }, {
            headers: {
              Authorization: `Bearer ${localStorage.getItem('token')}`
            }
          })
          console.log(response)
          this.canvasLogin = !this.canvasLogin
        } catch (error) {
          this.isLoading = false
          console.error(error)
          this.loadBackendData()
        }
      }
    },
    removeTask(index) {
      this.isLoading = true
      const taskId = this.tasks[index].id
      axios.delete(import.meta.env.VITE_API_KEY + `/api/ToDoTask/${taskId}`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`
        }
      }).then(response => { })
        .catch(error => {
          this.isLoading = false
          console.log(error)
          this.loadBackendData()
        })
    },
    toggleFavorite(index) {
      const task = this.tasks[index]
      task.isFavorite = !task.isFavorite
      const taskId = task.id

      this.isLoading = true

      if (this.isLoggedIn) {
        axios.put(import.meta.env.VITE_API_KEY + `/api/ToDoTask/${taskId}`, task, {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
          }
        }).then(response => { })
          .catch(error => {
            console.log(error)
            this.isLoading = false
            task.isFavorite = !task.isFavorite
            this.loadBackendData()
          })
      }
    },
    showTodo() {
      this.todoList = true
      this.completedList = false
    },
    showCompleted() {
      this.completedList = true
      this.todoList = false
    },
    toggleCompletedTask(index) {
      // this.isChecked[index] = !this.isChecked[index]
      this.fullDisplay[index] = ''
      const task = this.tasks[index]
      task.isCompleted = !task.isCompleted
      const taskId = task.id
      this.isLoading = true
      axios.put(import.meta.env.VITE_API_KEY + `/api/ToDoTask/${taskId}`, task, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`
        }
      }).then(response => { })
        .catch(error => {
          task.isCompleted = !task.isCompleted
          this.isLoading = false
          console.log(error)
          this.loadBackendData()
        })
    },
    showAllItemContents(index) {
      if (this.fullDisplay[index] === true) {
        this.fullDisplay[index] = ''
      } else {
        this.fullDisplay[index] = true
      }
    },
    toggleFormInput() {
      this.addItem = !this.addItem
    },
    toggleCanvasInput() {
      this.canvasLogin = !this.canvasLogin
    },
    loadTippySettings() {
      tippy('.tip', {
        theme: 'custom',
        arrow: true,
        placement: 'top'
      })
    },
    updateTitle(value) {
      this.newTask.title = value
    },
    updateDescription(value) {
      this.newTask.description = value
    },
    updateDueDate(value) {
      this.newTask.dueDate = value
    },
    setIsChecked(index) {
      this.isChecked[index] = true
    },
    removeIsChecked(index) {
      this.isChecked[index] = false
    }
  }
}
</script>

<style scoped>
.list-wrapper {
  display: flex;
  flex-direction: column;
  width: 500px;
  height: auto;
  margin: 0 auto;
  border: 0px solid black;
  border-radius: 10px;
  background-color: white;
  box-shadow: 25px 40px 28px 0px rgba(156, 156, 156, 0.38);
  margin-top: 60px;
  min-height: 657px;
}

.content-wrapper {
  min-height: 485px;
  max-height: 300px;
  overflow-y: scroll;
  position: relative;
  margin: 20px;
}

.completed-wrapper {
  min-height: 535px;
  max-height: 300px;
  overflow-y: scroll;
  position: relative;
  margin: 20px;
  margin-bottom: 100px;
}

.content-wrapper::-webkit-scrollbar {
  width: 8px;
}

.content-wrapper::-webkit-scrollbar-thumb {
  background-color: #F5F5F5;
  border-radius: 10px;
}

.content-wrapper::-webkit-scrollbar-thumb:hover {
  background-color: #999999;
}

.completed-wrapper::-webkit-scrollbar {
  width: 8px;
}

.completed-wrapper::-webkit-scrollbar-thumb {
  background-color: #F5F5F5;
  border-radius: 10px;
}

.completed-wrapper::-webkit-scrollbar-thumb:hover {
  background-color: #999999;
}

.wrapper {
  position: relative;
  height: calc(100vh - 60px);
}
</style>
