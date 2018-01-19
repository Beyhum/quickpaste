import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from '../../auth/auth.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'login',
  templateUrl: './login.component.html',
  styleUrls: ['../../shared/styles/validation.css'],
  styles: ["#register-prompt { text-align: center; }"]
})
export class LoginComponent implements OnInit {
  password: FormControl;
  username: FormControl;

  loginForm: FormGroup;

  constructor(private authService: AuthService, private router: Router, private toastr: ToastrService) {
    this.username = new FormControl('', [Validators.required, Validators.minLength(1)]);
    this.password = new FormControl('', [Validators.required, Validators.minLength(1)]);

    this.loginForm = new FormGroup({
      username: this.username,
      password: this.password
    });
  }

  login() {
    this.authService.loginUser(this.username.value, this.password.value).subscribe((loginSuccess: boolean) => {
      if (loginSuccess) {
        this.router.navigate(['/pastes']);
      } else {
        this.toastr.error('Invalid username/password');
      }
    });
  }

  ngOnInit() {

  }

}
