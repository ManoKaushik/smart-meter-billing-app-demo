import pika
import json
import random
import time
from datetime import datetime

# RabbitMQ setup
from datetime import datetime, timezone, timedelta
import json, pika, random, time

tz = timezone(timedelta(hours=5, minutes=30))
connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost',virtual_host='your_vhost',credentials=pika.PlainCredentials('your_username', 'your_password')))
channel = connection.channel()

exchange = 'your_exchange_name'
routing_key = 'your_routing_key'
channel.exchange_declare(exchange=exchange, exchange_type='direct', durable=True)

meter_ids = [
    "GN24A00187",
    "GN24A00193",
    "HP24E00301",
    "LT24C00255",
    "LT24I00419"
]

while True:
    for meter_id in meter_ids:
        reading = {
            "MeterId": meter_id,
            "ReadingDate": datetime.now(tz).replace(microsecond=0).isoformat(),
            "Voltage": round(random.uniform(210, 250), 2),
            "Current": round(random.uniform(5, 15), 2),
            "PowerFactor": round(random.uniform(0.8, 1.0), 2)
        }

        channel.basic_publish(
            exchange=exchange,
            routing_key=routing_key,
            body=json.dumps(reading)
        )

        print(f"Sent reading: {reading}")

    time.sleep(300)  # send new readings every 300 seconds

connection.close()
