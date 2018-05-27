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

    public loggedUser = "guest";

    private loggedIn: boolean;
    private failedLogin: boolean;
    private registered: boolean;
    private failedRegister: boolean;

    public receivedTickets = false;
    public tickets: Ticket[];

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {
        this.failedLogin = false;
        this.loggedIn = false;

        this.http.get(`${this.baseUrl}isLoggedIn`).subscribe(result => {
            this.loggedIn = result.text() == "true";

            if (!this.loggedIn) return;

            this.http.get(`${this.baseUrl}getLoggedUsername`).subscribe(result => {
                this.loggedUser = result.text();

                if (this.loggedUser == "guest") return;

                this.http.get(`${this.baseUrl}getTicketsByUser/?username=${this.loggedUser}`).subscribe(result => {
                    this.tickets = result.json() as Ticket[];
                    this.receivedTickets = true;
                }, error => console.error(error));
            }, error => console.error(error));

        }, error => console.error(error));
    }

    loginUser() {
        this.http.post(`${this.baseUrl}loginUser`, { username: this.username, password: this.password }).subscribe(result => {
            if (result.json()) {
                this.loggedIn = true;
                this.loggedUser = this.username;

                this.http.get(`${this.baseUrl}getTicketsByUser/?username=${this.loggedUser}`).subscribe(result => {
                    this.tickets = result.json() as Ticket[];
                    this.receivedTickets = true;
                }, error => console.error(error));
            }
            else {
                this.failedLogin = true;

                setTimeout(() => {
                    this.failedLogin = false;
                }, 5000);
            }
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
        this.http.post(this.baseUrl + 'logoutUser', {}).subscribe(result => {
            this.loggedIn = false;
        }, error => console.error(error));
    }
}

interface Ticket {
    distance: number;
    startStation: string;
    endStation: string;
}