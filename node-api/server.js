// Node.js Express API for Aspire Demo
//
// This is a simple Express API that demonstrates Aspire's Node.js support.
// It connects to Redis (if available) and exposes a simple HTTP API.

const express = require('express');
const app = express();

// Get Redis connection from Aspire's connection string injection
const redisConnection = process.env['ConnectionStrings__shared-cache'];
let redisClient = null;

if (redisConnection) {
    try {
        const redis = require('redis');
        redisClient = redis.createClient({ url: redisConnection });
        redisClient.connect().catch(console.error);
    } catch (e) {
        console.log('Could not connect to Redis:', e);
    }
}

app.get('/', (req, res) => {
    res.json({
        message: 'Hello from Node.js Express API!',
        managed_by: 'Aspire',
        conference: 'Swetugg Stockholm 2026'
    });
});

app.get('/health', (req, res) => {
    res.json({ status: 'healthy' });
});

app.get('/counter', async (req, res) => {
    if (redisClient) {
        try {
            const count = await redisClient.incr('node-counter');
            return res.json({ counter: count, source: 'redis' });
        } catch (e) {
            // Fall through
        }
    }
    res.json({ counter: 0, source: 'no-redis' });
});

const port = process.env.PORT || 3000;
app.listen(port, () => {
    console.log(`Node.js API listening on port ${port}`);
});
