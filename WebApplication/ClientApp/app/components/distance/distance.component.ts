import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'app-distance',
    templateUrl: './distance.component.html'
})
/** distance component*/
export class DistanceComponent {
    
    public stations: string[];
    public selectedStartStation: string;
    public selectedEndStation: string;

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {

        this.http.get(this.baseUrl + 'getAllStations').subscribe(result => {
            this.stations = result.json();
            this.selectedStartStation = this.stations[0];
            this.selectedEndStation = this.stations[0];
        }, error => console.error(error));
    }
    
    public distance = 0;

    public getDistance() {
        this.http.get(`${this.baseUrl}getDistance/?stationA=${this.selectedStartStation}&stationB=${this.selectedEndStation}` ).subscribe(result => {
            this.distance = result.json();
        }, error => console.error(error));
    }
}