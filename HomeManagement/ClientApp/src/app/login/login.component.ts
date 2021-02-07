import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private http: HttpClient) { }

  ngOnInit() {
  }

  userModel = new User("please enter username", "please enter password");
  onLogin() {
    console.log("hello");
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json' })
    }
    let json = JSON.stringify(this.userModel);
    this.http.post("https://localhost:44386/api/login", JSON.stringify(json), httpOptions)
      .subscribe();
  }
}
class User {
  constructor(
    public username: String,
    public password: String) {}
}
