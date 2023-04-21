<template>
  <div
    class="extension-wrapper"
    @click.self="$emit('toggle-extension-display')"
  >
    <div
      class="extensions-overlay"
    >
      <div
        class="close-icon"
        @click="$emit('toggle-extension-display')"
      >
        <span class="material-symbols-outlined closer">
          close
        </span>
      </div>
      <h4>Available Extensions</h4>
      <div @click="$emit('toggle-canvas-input'), $emit('toggle-extension-display')">
        <img
          class="canvas-icon can"
          data-tippy-content="Import Canvas Assignments"
          src="/images/canvas.png"
        >
      </div>
    </div>
  </div>
</template>

<script>
import tippy from 'tippy.js'
import 'tippy.js/dist/tippy.css'
export default {
  mounted () {
    tippy('.can', {
      theme: 'custom',
      arrow: true,
      placement: 'top'
    })
  }
}

</script>

<style scoped>
.extension-wrapper {
  position: absolute;
  width: 100%;
  top: 0px;
  right: 0px;
  height: 785px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.extensions-overlay{
  position: fixed;
  top: 48.3%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 598px;
  height: auto;
  box-shadow: 0px 2px 4px 0px rgba(156, 156, 156, 0.38);
  border: 1px solid rgba(156, 156, 156, 0.38);
  border-radius: 10px;
  background-color: white;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding-bottom: 50px;
}

h4 {
  margin: 0 auto;
  font-size: 25px;
  top: 220px;
  color: #6798ff;
  font-weight: normal;
  margin-top: 20px;
  margin-bottom: 50px;
}

.canvas-icon {
  height: 50px;
  width: 50px;
  cursor: pointer;
  border-radius: 50%;
  font-size: 30px;
  color: white;
  margin-right: 10px;
  margin: 0 auto;
  transition: all 0.3s ease;
    box-shadow: 5px 5px 10px #e0e0e0, -5px -5px 10px #ffffff;
  background-color: #ffffff;
  text-align: center;
}

.canvas-icon:hover {
  box-shadow: inset 5px 5px 10px #e0e0e0, inset -5px -5px 10px #ffffff;
  cursor: pointer;
}

.close-icon {
  position: absolute;
  right: 10px;
  top: 10px;
}

.closer {
  border: none;
  border-radius: 10px;
  padding: 3px;
  width: 40px;
  font-size: 12px;
  transition: all 0.3s ease;
    box-shadow: 5px 5px 10px #e0e0e0, -5px -5px 10px #ffffff;
  background-color: #ffffff;
  cursor: pointer;
    background-color: #6797ff00;
    font-size: 30px;
    color: rgb(0, 0, 0);
    text-align: center;

}

.closer:hover{
  box-shadow: inset 5px 5px 10px #e0e0e0, inset -5px -5px 10px #ffffff;
  cursor: pointer;
}

</style>
