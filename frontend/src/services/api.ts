import axios from 'axios';

export const api = axios.create({
    baseURL: 'http://localhost:5248/api', // Ajuste para a porta do seu backend C#
    headers: {
        'Content-Type': 'application/json',
    },
});