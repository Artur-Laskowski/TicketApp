import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { DistanceComponent } from './components/distance/distance.component';
import { UserComponent } from './components/user/user.component';
import { TicketComponent } from './components/ticket/ticket.component';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        DistanceComponent,
        UserComponent,
        TicketComponent,
        HomeComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'distance', component: DistanceComponent },
            { path: 'user', component: UserComponent },
            { path: 'ticket', component: TicketComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ]
})
export class AppModuleShared {
}
