<template>
  <div class="list-wrapper">
    <div class="header">
      <span class="material-symbols-outlined">
        sort
      </span>
      <h1>Todo</h1>
    </div>
    <div class="content-wrapper">
      <div
        v-for="(task, index) in tasks"
        :key="index"
      >
        <div class="list-item">
          <div class="listed">
            <span class="material-symbols-outlined smallCircle">
              circle
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
              <div @click="toggleFavorite(index), task.favorite = true">
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
    <div class="footer-wrapper">
      <div @click="addTask">
        <span class="material-symbols-outlined blue-circle">add</span>
      </div>
    </div>
  </div>
</template>

<script>
/*
    Todo:
*/

export default {
  data  () {
    return {
      newTask: '',
      tasks: [
        {
          title: 'Sample Task 1',
          date: new Date().toISOString().substring(0, 10),
          favorite: false
        },
        {
          title: 'Sample Task 2',
          date: new Date().toISOString().substring(0, 10),
          favorite: false
        },
        {
          title: 'Sample Task 3',
          date: new Date().toISOString().substring(0, 10),
          favorite: false
        },
        {
          title: 'Sample Task 4',
          date: new Date().toISOString().substring(0, 10),
          favorite: false
        }
      ]
    }
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
      this.tasks[index].favorite = !this.tasks[index].favorite
      if (this.tasks[index].favorite) {
        const task = this.tasks.splice(index, 1)
        this.tasks.unshift(task[0])
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
}

.header {
  display: flex;
  align-items: center;
  justify-content: right;
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
  margin-bottom: 50px;
  margin-top: 50px;
  display: flex;
  align-items: center;
  justify-content: right;
  margin-right: 50px;
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
}

</style>
