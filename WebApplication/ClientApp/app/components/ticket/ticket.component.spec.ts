/// <reference path="../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { TicketComponent } from './ticket.component';

let component: TicketComponent;
let fixture: ComponentFixture<TicketComponent>;

describe('ticket component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ TicketComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(TicketComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});