import { Component, OnInit, EventEmitter } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Router } from '@angular/router';
import { Local } from 'protractor/built/driverProviders';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {

  constructor(private cookieService: CookieService, private router: Router) {
    this.updater();
  }

  ngOnInit() {
    if (localStorage.getItem("login") != null) {
      let cookie = localStorage.getItem("login");
      const helper = new JwtHelperService();

      const decodedToken = helper.decodeToken(cookie);
      this.username = decodedToken.unique_name;
      this.userId = decodedToken.Id;
    }
  }

  username;
  userId;

  Logout() {
    localStorage.removeItem("login");
    this.router.navigate(["/login"]);
    this.username = null;
    this.userId = null;
  }

  updater() {
      setInterval(() => {
        this.ngOnInit();
      }, 100)
  }
}
