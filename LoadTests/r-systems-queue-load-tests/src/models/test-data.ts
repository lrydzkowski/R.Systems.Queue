import { SharedArray } from 'k6/data';

const config = new SharedArray('config', function () {
  return [JSON.parse(open('../../config/tests-config.json'))];
});

export const host = config[0].host;
