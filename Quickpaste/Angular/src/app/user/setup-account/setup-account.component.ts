import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { SetupAccountService } from './setup-account.service';
import { RegisterModel, RegisterResponse } from './register.model';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

/**
 * Component to initialize an account when none exists yet
 * 
 * @export
 * @class SetupAccountComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-setup-account',
  templateUrl: './setup-account.component.html',
  styleUrls: ['../../shared/styles/validation.css']

})
export class SetupAccountComponent implements OnInit {

  password: FormControl;
  passwordConfirmed: FormControl;
  username: FormControl;
  defaultUsername: FormControl;
  defaultPasscode: FormControl;

  setupAccountForm: FormGroup;

  constructor(private formBuilder: FormBuilder, private setupAccountService: SetupAccountService, private router: Router, private toastr: ToastrService) {
    this.initializeForm();
  }

  setupAccount() {
    let registerModel = new RegisterModel(this.defaultUsername.value, this.defaultPasscode.value, this.username.value, this.password.value);
    this.setupAccountService.register(registerModel).subscribe((resp: RegisterResponse) => {
      if (resp.success) {
        this.toastr.success(resp.display_text);
        this.router.navigate(['/pastes']);
      } else {
        this.toastr.error(resp.display_text);
      }
    });
  }

  PasswordValidator(formGroupControl: AbstractControl) {
    if (formGroupControl.get('password').value !== formGroupControl.get('confirm_password').value) {
      return { validatePasswords: 'Password must match confirmed password' };
    }
    return null;
  }

  private initializeForm() {
    this.username = new FormControl('', [Validators.required, Validators.minLength(1)]);
    this.password = new FormControl('', [Validators.required, Validators.minLength(1)]);
    this.passwordConfirmed = new FormControl('', [Validators.required, Validators.minLength(1)]);
    this.defaultUsername = new FormControl('', [Validators.required, Validators.minLength(1)]);
    this.defaultPasscode = new FormControl('', [Validators.required, Validators.minLength(1)]);

    this.setupAccountForm = this.formBuilder.group({
      default_passcode: this.defaultPasscode,
      default_username: this.defaultUsername,
      username: this.username,
      passwordGroup: this.formBuilder.group({
        password: this.password,
        confirm_password: this.passwordConfirmed
      }, { validator: this.PasswordValidator })
    });
  }
  
  ngOnInit() {
    
  }

}
