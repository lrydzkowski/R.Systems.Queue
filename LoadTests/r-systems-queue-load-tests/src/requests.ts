import { check } from 'k6';
import { Trend } from 'k6/metrics';
import http from 'k6/http';
// @ts-expect-error Import module
import { uuidv4 } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';

const requestTrends = {
  sendCompanyToQueue: new Trend('sendCompanyToQueue'),
  sendCompanyToTopic: new Trend('sendCompanyToTopic'),
};

export const sendCompanyToQueue = (host: string): void => {
  const url = `${host}/company/queue`;
  const payload = JSON.stringify({
    id: uuidv4(),
    name: 'test1name',
  });
  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };
  const response = http.post(url, payload, params);
  check(response, {
    'triggerJob response has status 200': (r) => r.status === 200,
  });
  requestTrends.sendCompanyToQueue.add(response.timings.duration);
};

export const sendCompanyToTopic = (host: string): void => {
  const url = `${host}/company/topic`;
  const payload = JSON.stringify({
    id: uuidv4(),
    name: 'test1name',
  });
  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };
  const response = http.post(url, payload, params);
  check(response, {
    'triggerJob response has status 200': (r) => r.status === 200,
  });
  requestTrends.sendCompanyToTopic.add(response.timings.duration);
};
