<template>
  <div class="list-wrapper">
    <div class="header">
      <ul
        v-show="tabs"
        v-if="tabs"
        class="tabs"
      >
        <li
          :class="{ 'gray-text': todoList, 'disable-pointer-events': todoList }"
          @click="showTodo"
        >
          Todo
        </li>
        <li
          :class="{ 'gray-text': completedList, 'disable-pointer-events': completedList }"
          @click="showCompleted"
        >
          Completed
        </li>
      </ul>
      <span
        class="material-symbols-outlined toggle-box"
      >
        sort
      </span>

      <h1 v-if="todoList">
        Todo
      </h1>
      <h1 v-else>
        Completed
      </h1>
    </div>
    <div
      v-if="todoList"
      class="content-wrapper"
    >
      <div
        v-for="(task, index) in tasks"
        :key="index"
      >
        <div
          v-if="task.completed === false"
          class="list-item"
        >
          <div class="listed">
            <span
              class="material-symbols-outlined smallCircle"
              :class="{ checked: task.isChecked }"
              @click="toggleCompletedTask(index)"
              @mouseenter="task.isChecked = true"
              @mouseleave="task.isChecked = false"
            >
              {{ task.isChecked ? 'check' : 'circle' }}
            </span>
            <div class="rowed-lines">
              <strong><span>{{ task.title }}</span></strong>
              <div class="full-line">
                <span>{{ task.date }}</span>
              </div>
            </div>
            <div class="icons">
              <div @click="removeTask(index)">
                <span class="material-symbols-outlined delete">delete</span>
              </div>
              <div @click="toggleFavorite(index)">
                <span
                  class="material-symbols-outlined"
                  :class="['favorite', { active: task.favorite }]"
                >favorite</span>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="footer-wrapper">
        <div @click="addTask">
          <span class="material-symbols-outlined blue-circle">add</span>
        </div>
      </div>
    </div>

    <div
      v-else
      class="content-wrapper"
    >
      <div
        v-for="(task, index) in tasks"
        :key="index"
      >
        <div
          v-if="task.completed === true"
          class="list-item"
        >
          <div class="listed">
            <span
              class="material-symbols-outlined smallCircle"
              :class="{ checked: task.isChecked }"
              @click="toggleCompletedTask(index)"
              @mouseenter="task.isChecked = false"
              @mouseleave="task.isChecked = true"
            >
              {{ task.isChecked ? 'check' : 'circle' }}
            </span>
            <div class="rowed-lines">
              <strong><span>{{ task.title }}</span></strong>
              <div class="full-line">
                <span>{{ task.date }}</span>
              </div>
            </div>
            <div class="icons">
              <div @click="removeTask(index)">
                <span class="material-symbols-outlined delete">delete</span>
              </div>
              <div @click="toggleFavorite(index)">
                <span
                  class="material-symbols-outlined"
                  :class="['favorite', { active: task.favorite }]"
                >favorite</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
/*
    Todo:
    un-favorite
    disable pointer on the boxes list selected tab
*/

export default {
  data  () {
    return {
      newTask: '',
      tasks: [
        {
          title: 'Sample Task 1',
          date: new Date().toISOString().substring(0, 10),
          favorite: false,
          completed: false,
          isChecked: false
        },
        {
          title: 'Sample Task 2',
          date: new Date().toISOString().substring(0, 10),
          favorite: false,
          completed: false,
          isChecked: false
        },
        {
          title: 'Sample Task 3',
          date: new Date().toISOString().substring(0, 10),
          favorite: false,
          completed: false,
          isChecked: false
        },
        {
          title: 'Sample Task 4',
          date: new Date().toISOString().substring(0, 10),
          favorite: false,
          completed: false,
          isChecked: false
        }
      ],
      tabs: true,
      todoList: true,
      completedList: false
    }
  },
  mounted () {
    setTimeout(() => {
      document.querySelector('.header ul').classList.add('show')
    }, 500)
  },
  methods: {
    addTask () {
      if (this.newTask.trim() !== '') {
        this.tasks.unshift({
          title: this.newTask,
          date: new Date().toISOString().substring(0, 10),
          favorite: false
        })
        this.newTask = ''
      }
    },
    removeTask (index) {
      this.tasks.splice(index, 1)
    },
    toggleFavorite (index) {
      const task = this.tasks[index]
      const originalIndex = this.tasks.findIndex(t => t === task)
      task.favorite = !task.favorite
      if (task.favorite) {
        // Move the task to the beginning of the list
        this.tasks.splice(originalIndex, 1)
        this.tasks.unshift(task)
      } else {
        // Move the task back to its original position
        this.tasks.splice(originalIndex, 1)
        this.tasks.splice(index, 0, task)
      }
    },
    showTodo () {
      this.todoList = true
      this.completedList = false
      this.animateList()
    },
    showCompleted () {
      this.completedList = true
      this.todoList = false
      this.animateList()
    },
    toggleCompletedTask (index) {
      this.tasks[index].completed = !this.tasks[index].completed
    },
    animateList () {
      const list = document.querySelector('.header ul')
      if (list) {
        list.classList.remove('show')
        setTimeout(() => {
          list.classList.add('show')
        }, 500)
      }
    }
  }
}
</script>

<style>
@import url('https://fonts.googleapis.com/css?family=Poppins&display=swap');

body {
  background: rgb(255,255,255);
  background: linear-gradient(90deg, rgba(255,255,255,1) 0%, rgba(228,228,242,1) 35%, rgba(249,252,255,1) 67%, rgba(241,252,255,1) 100%);
}

body, html {
  margin: 0;
  padding: 0;
}

* {
  font-family: Poppins;
}

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
  margin-top: 50px;
  min-height: 657px;
}

.header {
  display: flex;
  align-items: center;
  justify-content: right;
  position:relative
}

.header h1 {
  margin-top: 50px;
  margin-bottom: 50px;
  color: rgb(148, 148, 148);
  margin-right: 20px;
}

.header span {
  display: flex;
  align-items: center;
  justify-content: left;
  width: 100%;
  margin-left: 50px;
  font-size: 40px;
  color: #6798ff;
  cursor: default;
}

.header p{
  cursor: pointer;
  position: absolute;
  width: 40px;
  height: 40px;
  margin: 0px;
}

.favorite {
  color: rgb(0, 0, 0);
  cursor: pointer;
}

.delete {
  color: black;
  cursor: pointer;
}

.delete:hover {
  color: gray;
}

.favorite.active {
  color: red;
}

.rowed-lines {
  width: 100%;
  margin-left: 20px;
}

.list-item {
  display: flex;
  padding: 20px;
  border-radius: 20px;
  justify-content: center;
  flex-direction: column;
  align-items: left;
  height: auto;
  border: 0px solid black;
  margin-bottom: 20px;
  margin-left: 50px;
  margin-right: 50px;
  box-shadow: 0px 2px 4px 0px rgba(156, 156, 156, 0.38);
  border: 1px solid rgba(156, 156, 156, 0.38);
}

.footer-wrapper {
  margin-bottom: 20px;
  margin-top: 50px;
  display: flex;
  align-items: center;
  justify-content: right;
  margin-right: 50px;
  position: absolute;
  bottom: 0;
  right: 0px;
}

.footer-wrapper span {
  cursor: pointer;
  background-color: #6798ff;
  border-radius: 50%;
  font-size: 30px;
  padding: 10px;
  color: white;
  box-shadow: 0px 5px 10px 0px rgba(156, 156, 156, 0.38);
}

.full-line {
  display: flex;
  flex-direction: row;
}

.full-line span {
  width: 100%;
}

.content-wrapper {
  min-height: 500px;
  position:relative
}

.icons {
  display: flex;
  justify-content: right;
  align-items: right;
  width: 100%;
}

.listed {
  display: flex;
  flex-direction: row;
  align-items: center;
}

.listed .smallCircle {
  font-size: 15px;
  color: rgb(148, 148, 148);
  cursor: pointer;
}

.listed .smallCircle.checked {
  color: black;
  font-size: 15px;
  font-weight: bold;
  cursor: pointer;
}

.toggle-box {
  cursor: pointer;
}

.gray-text {
  color: gray;
  font-weight: bold;
}

.disable-pointer-events {
  pointer-events: none;
}

.tabs {
  text-transform: none;
  font-size: 12px;
  padding: 0px;
  padding-left: 5px;
  color: #6798ff;
  position: absolute;
  left: 100px;
}

.tabs li {
  text-decoration: none;
  list-style: none;
  margin-right: 10px;
  cursor: pointer;
}

.tabs li:hover {
  color: gray;
}

.header ul li {
  opacity: 0;
  transform: translateX(-30px);
  transition: opacity 0.3s ease-out, transform 0.3s ease-out;
}

.header ul.show li {
  opacity: 1;
  transform: translateX(0);
}

</style>
