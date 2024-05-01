import { NgIf } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Router, RouterOutlet , RouterLink} from '@angular/router';
import { AngularFontAwesomeModule } from 'angular-font-awesome';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterOutlet,NgIf,RouterLink  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

  constructor(private router: Router) { }

  // navigateToLogin() {
  //   this.router.navigate(['/user-dash']); // Navigate to the 'login' route
  // }
}
