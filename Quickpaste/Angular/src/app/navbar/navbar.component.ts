import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { PasteService } from '../pastes/paste.service';

@Component({
  selector: 'navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  isCollapsed: boolean = true;

  authService: AuthService;
  constructor(authService: AuthService, private router: Router, private pasteService: PasteService) {
    this.authService = authService;
   }

  logout() {
    let logoutSuccess = this.authService.logoutUser();
    if(logoutSuccess) {
      
      this.router.navigate(['/login']);
    }
  }

  /**
   * Toggles the navbar between collapsed and expanded view  on mobile display
   * 
   * @memberof NavbarComponent
   */
  toggleCollapse() {
    this.isCollapsed = !this.isCollapsed;
  }

  /**
   * Collapses the navbar on mobile display
   * 
   * @memberof NavbarComponent
   */
  collapse() {
    this.isCollapsed = true;
  }

  ngOnInit() {

  }

}
