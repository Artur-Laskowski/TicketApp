import { Component, Inject, Injectable } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'app-user',
    templateUrl: './user.component.html'
})
/** user component*/
@Injectable()
export class UserComponent {
    /** user ctor */
    public username: string;
    public password: string;

    private loggedIn: boolean;
    private failedLogin: boolean;
    private registered: boolean;
    private failedRegister: boolean;

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {
        this.failedLogin = false;

            this.loggedIn = false;
        
    }

    loginUser() {
        this.http.post(`${this.baseUrl}loginUser`, { username: this.username, password: this.password }).subscribe(result => {
            console.log("qwer");
            if (true) {
                console.log(result);
                //localStorage.setItem('currentUser', result.json());
                this.loggedIn = true;
            }
            /*else {
                console.log("adsf2");
                this.failedLogin = true;

                setTimeout(() => {
                    this.failedLogin = false;
                }, 5000);
            }*/
        }, error => console.error(error));
    }

    registerUser() {
        this.http.post(this.baseUrl + 'registerUser', { username: this.username, password: this.password }).subscribe(result => {
            if (result.json()) {
                this.registered = true;

                setTimeout(() => {
                    this.registered = false;
                }, 5000);
            }
            else {
                this.failedRegister = true;

                setTimeout(() => {
                    this.failedRegister = false;
                }, 5000);
            }
        }, error => console.error(error));
    }

    logout() {
        //localStorage.removeItem('currentUser');
        this.loggedIn = false;
    }
}