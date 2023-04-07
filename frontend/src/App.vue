<template>
  <div class="wrapper">
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
      <div v-if="todoList">
        <div
          class="content-wrapper"
        >
          <div
            v-for="(task, index) in tasks"
            :key="index"
          >
            <div
              v-if="task.isCompleted === false"
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
                <div
                  v-if="fullDisplay[index] !== false"
                  class="rowed-lines"
                  @click="showAllItemContents(index)"
                >
                  <strong v-if="fullDisplay[index] !== true"><span>{{ task.title }}</span></strong>
                  <div class="full-line">
                    <strong><span v-if="fullDisplay[index]">{{ task.title }}</span></strong>
                    <span v-if="fullDisplay[index]"><strong>Created: </strong>{{ task.created }}</span>
                    <span v-if="fullDisplay[index]"><strong>Due Date: </strong>{{ task.dueDate }}</span>
                    <span v-if="fullDisplay[index] !==true">Due Date: {{ task.dueDate }}</span>
                    <span v-if="fullDisplay[index]"><br><strong>Description: </strong>{{ task.description }}</span>
                  <!--
                  <span v-if="fullDisplay[index]"><br><strong>Updated: </strong>{{ task.updated }}</span>
                  -->
                  </div>
                </div>
                <div
                  v-if="fullDisplay[index] !== true"
                  class="icons"
                >
                  <div @click="removeTask(index)">
                    <span class="material-symbols-outlined delete">delete</span>
                  </div>
                  <div @click="toggleFavorite(index)">
                    <span
                      class="material-symbols-outlined"
                      :class="['favorite', { active: task.isFavorite }]"
                    >favorite</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="footer-wrapper">
          <div @click="toggleFormInput">
            <span class="material-symbols-outlined blue-circle">add</span>
          </div>
        </div>
      </div>

      <div
        v-else
        class="completed-wrapper"
      >
        <div
          v-for="(task, index) in tasks"
          :key="index"
        >
          <div
            v-if="task.isCompleted === true"
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
              <div
                v-if="fullDisplay[index] !== false"
                class="rowed-lines"
                @click="showAllItemContents(index)"
              >
                <strong v-if="fullDisplay[index] !== true"><span>{{ task.title }}</span></strong>
                <div class="full-line">
                  <strong><span v-if="fullDisplay[index]">{{ task.title }}</span></strong>
                  <span v-if="fullDisplay[index]"><strong>Created: </strong>{{ task.created }}</span>
                  <span v-if="fullDisplay[index]"><strong>Due Date: </strong>{{ task.dueDate }}</span>
                  <span v-if="fullDisplay[index]"><br><strong>Description: </strong>{{ task.description }}</span>
                <!--
                  <span v-if="fullDisplay[index]"><br><strong>Updated: </strong>{{ task.updated }}</span>
                  -->
                </div>
              </div>
              <div
                v-if="fullDisplay[index] !== true"
                class="icons"
              >
                <div @click="removeTask(index)">
                  <span class="material-symbols-outlined delete">delete</span>
                </div>
                <div @click="toggleFavorite(index)">
                  <span
                    class="material-symbols-outlined"
                    :class="['favorite', { active: task.isFavorite }]"
                  >favorite</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div
      v-if="addItem==true"
      class="form-wrapper"
    >
      <form @submit.prevent="submitForm">
        <div
          class="close-form"
          @click="toggleFormInput"
        >
          <span class="material-symbols-outlined">
            close
          </span>
        </div>
        <label for="title">Title</label>
        <input
          id="title"
          v-model="newTask.title"
          type="text"
          required
        >
        <br>
        <label for="description">Description</label>
        <textarea
          id="description"
          v-model="newTask.description"
        />
        <br>
        <label for="dueDate">Due Date</label>
        <input
          id="dueDate"
          v-model="newTask.dueDate"
          type="date"
        >
        <br>
        <div class="submit-section">
          <button type="submit">
            <div
              class="form-submit"
            >
              <span class="material-symbols-outlined blue-circle">add</span>
            </div>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
/*
    Todo:
    add edit button on full view
*/

export default {
  data  () {
    return {
      newTask: {
        title: '',
        description: '',
        dueDate: ''
      },
      tasks: [
        {
          id: 1,
          title: 'Sample Task 1',
          description: 'Dtricies tristique nulla aliquet enim. Sed ullamcorper morbi tincidunt ornare massa eget egestas purus. Condimentum lacinia quis vel eros donec ac odio tempor. Mattis pellentesque id nibh tortor. Eu sem integer vitae justo eget magna. Quisque egestas diam in arcu cursus euismod quis viverra. Nibh tortor id aliquet lectus proin nibh nisl condimentum id. Morbi tristique senectus et netus et. Cras pulvinar mattis nunc sed blandit libero volutpat sed cra.',
          created: new Date().toISOString().substring(0, 10),
          dueDate: new Date().toISOString().substring(0, 10),
          updated: '',
          taskType: 0,
          isFavorite: false,
          isCompleted: false,
          isChecked: false
        }
      ],
      tabs: true,
      todoList: true,
      completedList: false,
      addItem: false,
      fullDisplay: []
    }
  },
  mounted () {
    setTimeout(() => {
      document.querySelector('.header ul').classList.add('show')
    }, 200)
  },
  methods: {
    submitForm () {
      // Set created date to current date
      this.newTask.created = new Date().toISOString().substring(0, 10)
      // Add new task to tasks array
      this.tasks.unshift({
        id: this.tasks.length + 1,
        title: this.newTask.title,
        description: this.newTask.description,
        created: this.newTask.created,
        dueDate: this.newTask.dueDate,
        updated: '',
        taskType: 0,
        isFavorite: false,
        isCompleted: false,
        isChecked: false
      })
      // Reset form fields
      this.newTask.title = ''
      this.newTask.description = ''
      this.newTask.dueDate = ''
      this.addItem = false
    },
    removeTask (index) {
      this.tasks.splice(index, 1)
    },
    toggleFavorite (index) {
      const task = this.tasks[index]
      task.isFavorite = !task.isFavorite
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
      this.tasks[index].isCompleted = !this.tasks[index].isCompleted
    },
    animateList () {
      const list = document.querySelector('.header ul')
      if (list) {
        list.classList.remove('show')
        setTimeout(() => {
          list.classList.add('show')
        }, 200)
      }
    },
    showAllItemContents (index) {
      if (this.fullDisplay[index] === true) {
        this.fullDisplay[index] = ''
      } else {
        this.fullDisplay[index] = true
      }
    },
    toggleFormInput () {
      this.addItem = !this.addItem
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
  margin-top: 60px;
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
  cursor: pointer;
}

.rowed-lines:hover {
  opacity: .9;
 /* color: #6798ff; */
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
  margin-left: 35px;
  margin-right: 25px;
  box-shadow: 0px 2px 4px 0px rgba(156, 156, 156, 0.38);
  border: 1px solid rgba(156, 156, 156, 0.38);
}

.footer-wrapper {
  display: flex;
  justify-content: right;
  margin-bottom: 50px;
  margin-top: 50px;
  margin-right: 65px;
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

.form-submit span {
  cursor: pointer;
  background-color: #6798ff;
  border-radius: 50%;
  font-size: 30px;
  padding: 10px;
  color: white;
  box-shadow: 0px 5px 10px 0px rgba(156, 156, 156, 0.38);
}

.form-submit span:hover {
  background-color: #83aaff;
}

.footer-wrapper span:hover {
  background-color: #83aaff;
}

.full-line {
  display: flex;
  flex-direction: column;
  width: 100%;
}

.full-line span {
  width: 100%;
}

.content-wrapper {
  min-height: 480px;
  max-height: 300px;
  overflow-y: scroll;
  position:relative;
  margin: 20px;
}

.completed-wrapper {
  min-height: 530px;
  max-height: 300px;
  overflow-y: scroll;
  position:relative;
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

.icons {
  display: flex;
  justify-content: right;
  align-items: right;
  width: auto;
  margin-left: 15px;
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
  transform: translateX(-20px);
  transition: all 0.3s;
}

.header ul.show li {
  opacity: 1;
  transform: translateX(0);
}

.wrapper {
  position: relative;
}

.form-wrapper {
  position: absolute;
  width: 100%;
  height: auto;
  top: 200px;

}

.form-wrapper form {
  position: relative;
  z-index:1000;
  margin: 0 auto;
  box-shadow: 0px 2px 4px 0px rgba(156, 156, 156, 0.38);
  border: 1px solid rgba(156, 156, 156, 0.38);
  border-radius: 10px;
  background-color: white;
  padding: 20px;
  padding-left: 100px;
  padding-right: 100px;
  width: 400px;
  height: 350px;
  display: flex;
  justify-content: center;
  align-items: left;
  flex-direction: column;
  font-weight: bold;
}

.form-wrapper form input, textarea {
  box-shadow: 0px 2px 4px 0px rgba(156, 156, 156, 0.38);
  border: 1px solid rgba(156, 156, 156, 0.38);
}

form label {
  margin-bottom: 5px;
}

textarea:focus {
  outline: 2px solid #6798ff;
}

input {
  height: 30px;
  border-radius: 10px;
  font-size: 12px;
}

textarea {
  height: 30px;
  border-radius: 10px;
  font-size: 12px;
}

input:focus {
  outline: 2px solid #6798ff;
}

form button {
  border: transparent;
  background-color: white;
}

form button {
  width: 50px;
  height: 50px;
  padding: 0px;
}

.submit-section {
  margin-top: 20px;
  display: flex;
  justify-content: center;
}

.form-submit {
  width: 50px;
  height: 50px;
}

.close-form {
  display: flex;
  justify-content: right;
  height: auto;
  position: absolute;
  top: 10px;
  right: 10px;
}

.close-form span{
  cursor: pointer;
  background-color: #6797ff00;
  font-size: 30px;
  color: rgb(0, 0, 0);
}

</style>
