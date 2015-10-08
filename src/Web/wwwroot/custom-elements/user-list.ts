import {customElement, bindable} from 'aurelia-framework';
import {IPerson} from 'voting';

@customElement('user-list')
export class UserList {
  @bindable users: IPerson[];

}