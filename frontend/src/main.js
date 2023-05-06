import { createApp } from 'vue'
import App from './App.vue'
import { VueSignalR } from '@dreamonkey/vue-signalr'
import { HubConnectionBuilder } from '@microsoft/signalr'

const connection = new HubConnectionBuilder()
    .withUrl(import.meta.env.VITE_API_KEY + '/UserNotification', {
        accessTokenFactory: () => localStorage.getItem('token')
    })
    .withAutomaticReconnect()
    .build()

createApp(App).use(VueSignalR, { connection }).mount('#app')
