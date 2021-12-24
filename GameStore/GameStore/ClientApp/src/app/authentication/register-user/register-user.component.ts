import { UserForRegistrationDto } from "../../_interfaces/user/userForRegistrationDto";
import { AuthenticationService} from "../../../shared/services/authentication.service";
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import {
  PasswordConfirmationValidatorService
} from "../../../shared/custom-validators/password-confirmation-validator.service";

@Component({
  selector: 'app-register-user',
  templateUrl: './register-user.component.html',
  styleUrls: ['./register-user.component.css']
})
export class RegisterUserComponent implements OnInit {

  public registerForm: FormGroup;
  public errorMessage:string = '';
  public showError:boolean;

  constructor(private _authService: AuthenticationService, private _passConfValidator:PasswordConfirmationValidatorService) { }

  ngOnInit(): void {
    this.registerForm = new FormGroup({
      firstName: new FormControl(''),
      lastName: new FormControl(''),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required]),
      confirmPassword: new FormControl('', [Validators.required])
    });
    this.registerForm.get('confirmPassword').setValidators([Validators.required,
    this._passConfValidator.validateConfirmPassword(this.registerForm.get('password'))]);
  }

  public validateControl = (controlName:string) => {
    return this.registerForm.controls[controlName].invalid && this.registerForm.controls[controlName].touched;
  }
  public hasError = (controlName:string, errorName: string) => {
    return this.registerForm.controls[controlName].hasError(errorName);
  }
  public registerUser = (registerFormValue) => {
    this.showError = false;
    const formValues = {...registerFormValue};

    const user: UserForRegistrationDto = {
      firstName: formValues.firstName,
      lastName: formValues.lastName,
      email: formValues.email,
      password: formValues.password,
      confirmPassword: formValues.confirmPassword
    };

    this._authService.registerUser("api/accounts/registration", user)
      .subscribe(_ =>{
        console.log("Success")
      }, error =>{
        this.errorMessage = error;
        this.showError = true;
      })
    }
}
