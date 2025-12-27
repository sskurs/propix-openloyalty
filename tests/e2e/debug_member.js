const axios = require('axios');

const API = 'http://127.0.0.1:5122/api';
const email = `debug-${Date.now()}@example.com`;

async function run() {
    try {
        console.log('Creating member:', email);
        const res = await axios.post(`${API}/members`, {
            firstName: "Debug",
            lastName: "User",
            email: email,
            phoneNumber: "555-0000",
            hasAgreedToTerms: true
        });
        console.log('Success:', res.status, res.data);
    } catch (e) {
        console.error('Error Status:', e.response?.status);
        console.error('Error Data:', e.response?.data);
        console.error('Message:', e.message);
    }
}

run();
