import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { MybookingsComponent } from './pages/mybookings/mybookings.component';
import { NeweventComponent } from './pages/newevent/newevent.component';
import { EventlistComponent } from './pages/eventlist/eventlist.component';
import { UserdashComponent } from './pages/userdash/userdash.component';
import { RegisterComponent } from './pages/register/register.component';
import { LoginComponent } from './pages/login/login.component';
import { EventbookingComponent } from './pages/eventbooking/eventbooking.component';
import { NavbarComponent } from './pages/navbar/navbar.component';
import { UserprofileComponent } from './pages/userprofile/userprofile.component';
import { SidebarComponent } from './pages/sidebar/sidebar.component';
import { MyeventsComponent } from './pages/myevents/myevents.component';

export const routes: Routes = [
    {
        path:'',
        redirectTo:'Home',
        pathMatch:'full'
    },
    {
        path:'Home',
        component:HomeComponent
    },
    {
        path:'new-event',
        component:NeweventComponent
    },
    {
        path:'event-bookings',
        component:EventbookingComponent
    },
    {
        path:'event-list',
        component:EventlistComponent
    },
    {
        path:'user-dash',
        component:UserdashComponent,
        children: [
            { path: 'user-profile', component: UserprofileComponent },
            { path:'mybookings', component:MybookingsComponent },
          ]
    },
    {
        path:'register',
        component:RegisterComponent
    },
    {
        path:'login',
        component:LoginComponent
    },
    {
        path:'navbar',
        component:NavbarComponent
    },
    {
        path:'sidebar',
        component:SidebarComponent
    },
    {
        path:'my-events',
        component:MyeventsComponent
    }
];

