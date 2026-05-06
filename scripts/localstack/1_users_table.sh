#!/bin/bash

TABLE='auth-dev-users'
SEEDS_DIR='/seeds'

awslocal dynamodb create-table \
  --table-name "$TABLE" \
  --attribute-definitions \
    AttributeName=id,AttributeType=S \
    AttributeName=cpfNumber,AttributeType=S \
  --key-schema AttributeName=id,KeyType=HASH \
  --global-secondary-indexes '[
    {
      "IndexName": "cpfNumber-index",
      "KeySchema": [{"AttributeName": "cpfNumber", "KeyType": "HASH"}],
      "Projection": {"ProjectionType": "ALL"}
    }
  ]' \
  --billing-mode PAY_PER_REQUEST > /dev/null
echo "Table '$TABLE' created."

seed() {
python3 -c "
import json, subprocess, sys

with open('$1', encoding='utf-8-sig') as f:
    users = json.load(f)

for user in users:
    item = {k: {'S': v} for k, v in user.items()}
    result = subprocess.run([
        'awslocal', 'dynamodb', 'put-item',
        '--table-name', '$TABLE',
        '--item', json.dumps(item)
    ], capture_output=True)

    label = user['fullName']
    if result.returncode == 0:
        print(f\"User '{label}' seeded.\")
"
}

seed "$SEEDS_DIR/users.json"
seed "$SEEDS_DIR/users-test.json"
