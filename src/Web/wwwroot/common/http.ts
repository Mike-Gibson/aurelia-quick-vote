import {HttpClient} from 'aurelia-http-client';

var http = new HttpClient();

http.configure(x=> {
  x.withBaseUrl('');
  x.withHeader('content-type', 'application/json')
});

export default http;