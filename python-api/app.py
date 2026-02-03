# Python Flask API for Aspire Demo
# 
# This is a simple Flask API that demonstrates Aspire's Python support.
# It connects to Redis (if available) and exposes a simple HTTP API.

from flask import Flask, jsonify
import os
import redis

app = Flask(__name__)

# Get Redis connection from Aspire's connection string injection
redis_connection = os.environ.get('ConnectionStrings__shared-cache', None)
redis_client = None

if redis_connection:
    try:
        # Parse connection string (format: host:port)
        host, port = redis_connection.replace('tcp://', '').split(':')
        redis_client = redis.Redis(host=host, port=int(port), decode_responses=True)
    except Exception as e:
        print(f"Could not connect to Redis: {e}")

@app.route('/')
def home():
    return jsonify({
        "message": "Hello from Python Flask API!",
        "managed_by": "Aspire",
        "conference": "Swetugg Stockholm 2026"
    })

@app.route('/health')
def health():
    return jsonify({"status": "healthy"})

@app.route('/counter')
def counter():
    if redis_client:
        try:
            count = redis_client.incr('python-counter')
            return jsonify({"counter": count, "source": "redis"})
        except:
            pass
    return jsonify({"counter": 0, "source": "no-redis"})

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 5000))
    app.run(host='0.0.0.0', port=port)
