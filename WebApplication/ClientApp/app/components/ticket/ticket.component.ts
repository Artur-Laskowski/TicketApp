import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'app-ticket',
    templateUrl: './ticket.component.html'
})
/** ticket component*/
export class TicketComponent {

    public stations: string[];
    public selectedStartStation: string;
    public selectedEndStation: string;
    public loggedIn: boolean;
    public bought: boolean;
    public failure: boolean;

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {

        this.http.get(this.baseUrl + 'getAllStations').subscribe(result => {
            this.stations = result.json();
            this.selectedStartStation = this.stations[0];
            this.selectedEndStation = this.stations[0];
        }, error => console.error(error));

        this.http.get(`${this.baseUrl}isLoggedIn`, {}).subscribe(result => {
            this.loggedIn = result.text() == "true";
        }, error => console.error(error));


    }

    public price = 0;

    public getDistance() {
        this.http.get(`${this.baseUrl}getDistance/?stationA=${this.selectedStartStation}&stationB=${this.selectedEndStation}`).subscribe(result => {
            this.price = result.json();
        }, error => console.error(error));
    }

    public buyTicket() {
        this.http.post(this.baseUrl + 'buyTicket', { stationA: this.selectedStartStation, stationB: this.selectedEndStation }).subscribe(result => {
            if (result.json()) {
                this.bought = true;

                setTimeout(() => {
                    this.bought = false;
                }, 5000);
            }
            else {
                this.failure = true;

                setTimeout(() => {
                    this.failure = false;
                }, 5000);
            }
        }, error => console.error(error));
    }
}