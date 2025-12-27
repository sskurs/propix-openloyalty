@echo off
curl -X POST http://localhost:5122/api/pos/transaction ^
-H "Content-Type: application/json" ^
-d "{ \"eventType\": \"transaction.recorded\", \"customerId\": \"CUST-1001\", \"transactionId\": \"TXN-98765\", \"storeId\": \"STORE-01\", \"sellerId\": \"EMP-220\", \"currency\": \"INR\", \"totalAmount\": 2500, \"items\": [ { \"sku\": \"SKU-101\", \"qty\": 2, \"price\": 400 }, { \"sku\": \"SKU-555\", \"qty\": 1, \"price\": 1700 } ], \"timestamp\": \"2025-12-24T12:10:00Z\", \"channel\": \"POS\" }"
