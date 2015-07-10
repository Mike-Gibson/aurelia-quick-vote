import {computedFrom} from 'aurelia-framework';
import {inject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-http-client';

@inject(HttpClient)
export class Welcome{
  name: string;
  
  private http: HttpClient;

  constructor(http: HttpClient) {
    this.http = http;
    this.name = '';
  }
  
  login() {
    this.http.post('http://localhost:5000/api/login', 'mike').then(response => {
      alert('YAY')
    }, error => {
      alert('boo :(');
      console.log(error);
    });
  }  
}