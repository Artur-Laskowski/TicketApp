import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'app-distance',
    templateUrl: './distance.component.html'
})
/** distance component*/
export class DistanceComponent {
    /** distance ctor */

    public httpv: Http;
    public baseUrlv: string;
    public stations: string[];
    public selectedStartStation: string;
    public selectedEndStation: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.httpv = http;
        this.baseUrlv = baseUrl;

        this.httpv.get(this.baseUrlv + 'getAllStations').subscribe(result => {
            this.stations = result.json();
            this.selectedStartStation = this.stations[0];
            this.selectedEndStation = this.stations[0];
        }, error => console.error(error));
    }

    public currentCount = 0;
    public distance = 0;

    public incrementCounter() {
        this.currentCount++;
    }

    public getDistance() {
        this.httpv.get(this.baseUrlv + 'getDistance/?stationA=' + this.selectedStartStation + '&stationB=' + this.selectedEndStation ).subscribe(result => {
            this.distance = result.json();
        }, error => console.error(error));
    }
}