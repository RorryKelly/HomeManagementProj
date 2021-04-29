import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-init',
  templateUrl: './init.component.html',
  styleUrls: ['./init.component.css']
})
export class InitComponent implements OnInit {

  constructor(public router: Router, private cookieService: CookieService) {
    this.CheckLogin();
  }

  ngOnInit() {
  }

  CheckLogin() {
    let isLoggedIn = localStorage.getItem("login") != null;
    if (isLoggedIn)
      this.router.navigate(['home']);
    else
      this.router.navigate(['login']);
  }
}
