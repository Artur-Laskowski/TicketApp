import { Component, Inject, Injectable } from '@angular/core';
import { Http } from '@angular/http';
import * as $ from 'jquery';
import * as signalR from '@aspnet/signalr';
import { HubConnectionBuilder, HubConnection } from '@aspnet/signalr';

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
    public ticketsAsync: Ticket[];
    public connection: HubConnection;

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {
        this.failedLogin = false;
        this.loggedIn = false;

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/TicketHub")
            .build();

        this.connection.on("SendTickets", data => {
            console.log(data);
            this.ticketsAsync = data as Ticket[];
        });

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
                
                this.connection.start()
                    .then(() => this.connection.invoke("RequestTickets", this.loggedUser));

            }, error => console.error(error));

        }, error => console.error(error));
    }

    loginUser() {
        this.http.post(`${this.baseUrl}loginUser`, { username: this.username, password: this.password }).subscribe(result => {
            if (result.text() == "true") {
                this.loggedIn = true;
                this.loggedUser = this.username;
                
                this.connection.start()
                    .then(() => this.connection.invoke("RequestTickets", this.loggedUser));

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
            if (result.text() == "true") {
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