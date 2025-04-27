import { Options } from 'k6/options';
import { host } from './models/test-data.js';
import { sendCompanyToQueue, sendCompanyToTopic } from './requests.js';
// @ts-expect-error Import module
import { htmlReport } from 'https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js';

export const options: Options = {
  scenarios: {
    scenario1: {
      executor: 'ramping-vus',
      exec: 'scenario1',
      stages: [
        { duration: '20s', target: 10 },
        { duration: '30s', target: 10 },
        { duration: '10s', target: 0 },
      ],
    },
  },
};

export function scenario1() {
  sendCompanyToQueue(host);
  sendCompanyToTopic(host);
}

export const handleSummary = function (data: unknown) {
  return {
    './results/summary.html': htmlReport(data, { title: new Date().toLocaleString() }),
    './results/summary.json': JSON.stringify(data),
  };
};
