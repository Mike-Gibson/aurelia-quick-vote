import {computedFrom} from 'aurelia-framework';
import {inject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';

@inject(HttpClient)
export class Welcome{
  name: string;
  
  private http: HttpClient;

  constructor() {
    this.http = new HttpClient().configure(x=> {
      x.withBaseUrl('http://localhost:9001/');
      x.withHeader('content-type', 'application/json')
    });
    this.name = '';
  }
  
  login() {
    this.http.post('api/login/login', '"' + this.name + '"').then(response => {
      alert('YAY')
    }, error => {
      alert('boo :(');
      console.log(error);
    });
  }  
}