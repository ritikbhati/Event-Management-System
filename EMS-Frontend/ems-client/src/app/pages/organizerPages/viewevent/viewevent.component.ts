import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { UserdataService } from '../../../services/userDataService/userdata.service';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-viewevent',
  standalone: true,
  imports: [NgIf, NgFor, NgClass, FormsModule],
  templateUrl: './viewevent.component.html',
  styleUrl: './viewevent.component.css'
})
export class VieweventComponent implements OnInit{
  @ViewChild('scrollContainer') scrollContainer: ElementRef | undefined;
  eventId : number = 0;
  eventDetails : any;
  displayedUsers: any = []; // Users to be displayed
  allUsers: any = []; // All users
  pageSize: number = 5; // Number of users to load at a time
  isLoading: boolean = false; // Flag to track loading state
  allUsersLoaded: boolean = false; // Flag to track if all users are loaded
  showForm: boolean = false;
  emailSubject: string = '';
  emailMessage: string = '';
  emailResponse: string = '';

  constructor(private userdataService : UserdataService) {}

  ngOnInit(): void {
    this.eventId = this.userdataService.eventId;
    if (this.eventId) {
      this.userdataService.trackTicketDetails(this.eventId).subscribe(
        (eventDetails: any) => {
          this.eventDetails = eventDetails;
          console.log("ticket and event details", this.eventDetails['bookedUsers']);
          this.allUsers = eventDetails['bookedUsers'];
          console.log('all users', this.allUsers);
          this.loadUsers();
        },
        (error) => {
          console.error("Error fetching event details:", error);
        }
      );
    }
    // this.allUsers = this.eventDetails.bookedUsers;

  }
  isEventDone(): boolean {
    const eventDate = new Date(this.eventDetails.eventDate);
    const today = new Date();
    return eventDate < today; // Returns true if event date is before today
  }
  calculateTotalRevenue(): number {
    let totalRevenue = 0;
    if (this.eventDetails && this.eventDetails.bookedUsers) {
      for (const user of this.eventDetails.bookedUsers) {
        totalRevenue += user.totalPrice;
      }
    }
    return totalRevenue;
  }
  loadUsers(): void {
    const startIndex = this.displayedUsers.length;
    const endIndex = startIndex + this.pageSize;
    this.displayedUsers = this.allUsers.slice(0, endIndex);
    if (this.displayedUsers.length === this.allUsers.length) {
      this.allUsersLoaded = true;
    }
  }

  loadMoreUsers(): void {
    const startIndex = this.displayedUsers.length;
    const endIndex = startIndex + this.pageSize;
    if (endIndex <= this.allUsers.length) {
      this.displayedUsers = this.displayedUsers.concat(this.allUsers.slice(startIndex, endIndex));
    }
    if (this.displayedUsers.length === this.allUsers.length) {
      this.allUsersLoaded = true;
    }
  }
  showEmailForm() {
    this.showForm = true;
  }
  hideEmailForm() {
    this.emailSubject='';
    this.emailMessage='';
    this.showForm = false;
  }
  sendEmail() {
    // Prepare data
    const emailData = {
      Subject: this.emailSubject,
      Message: this.emailMessage
    };
    console.log('email data',emailData)
    this.userdataService.sendEmailNotifications(this.eventId, emailData).subscribe(response => {
        console.log(response);
        this.hideEmailForm();
      }, error => {
        console.error(error);
      });
  }
}