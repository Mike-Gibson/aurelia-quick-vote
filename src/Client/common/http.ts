import {HttpClient} from 'aurelia-http-client';

var http = new HttpClient();

http.configure(x=> {
  x.withBaseUrl('http://localhost:9001/');
  x.withHeader('content-type', 'application/json')
});

export default http;