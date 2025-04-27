# r-systems-queue-load-tests

Load tests written in TypeScript with Grafana k6.

It contains one load test that is sending requests to API.

## Prerequisites

1. Visual Studio Code
2. k6 - <https://grafana.com/docs/k6/latest/set-up/install-k6/>
3. NodeJS 22 - <https://nodejs.org/en>

## How to run it

1. Create `./config/tests-config.json` based on `./config/tests-config.json_sample`. You should fill in the host of tested API.
2. Run the following commands:

   ```powershell
   npm install
   npm run test1
   ```

3. Open summary: `./results/summary.html`.
