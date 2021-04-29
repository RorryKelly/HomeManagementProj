import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private cookieService: CookieService, private http: HttpClient, private router: Router) { }

  ngOnInit() {
  }

  hasFailed = false;
  errorMessage: string;

  onLogin(login) {
    let userModel = new User(login.controls.username.value, login.controls.password.value);
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
      'responseType': 'text' as 'json'
    };
    let json = JSON.stringify(userModel);
    this.http.post("https://localhost:44352/api/login", JSON.stringify(json), httpOptions)
      .subscribe({
        next: (x) => { this.saveToken(x); },
        error: err => {
          this.hasFailed = true;
          this.errorMessage = err.error;
        }
      })
  }

  saveToken(token) {
    localStorage.setItem("login", token);
    this.router.navigate(['/home']);
  }
}
class User {
  constructor(
    public username: String,
    public password: String) {}
}
