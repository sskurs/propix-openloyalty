@echo off
echo ==========================================
echo Starting End-to-End Verification Script
echo ==========================================

echo [1/2] Creating 'Welcome Coupon Campaign' via Admin API...
REM Assuming campaign_payload.json exists from previous step
curl -X POST http://localhost:5122/api/campaigns -H "Content-Type: application/json" -d @campaign_payload.json
echo.
echo.

echo [2/2] Simulating Transaction (ID: tx-end-2-end-5) via Kafka...
REM Using 'type' command to pipe file content avoiding shell escaping issues
type transaction_payload.json | docker exec -i kafka kafka-console-producer --bootstrap-server localhost:9092 --topic transactions
echo.

echo ==========================================
echo Done! 
echo Check Worker logs to confirm 'Coupon Issued':
echo docker logs worker-engine-worker-1
echo ==========================================
pause
