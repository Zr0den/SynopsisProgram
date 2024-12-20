import http from 'k6/http'; 
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '30s', target: 10 }, // 10 users for 30 seconds
        { duration: '1m', target: 50 },  // Ramp up to 50 users for 1 minute
        { duration: '30s', target: 0 },  // Ramp down to 0 users over 30 seconds
    ],
};

export default function () {
    let response = http.get('http://localhost:5000/api/catalog/products?productIds=1,2');

    check(response, {
        'is status 200': (r) => r.status === 200,
    });

    // Small pause between requests to simulate user behavior :)
    sleep(1);
}