import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {HttpClientModule} from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { BsModalModule } from 'ng2-bs3-modal';

import { AppComponent } from './app.component';
import { NavbarComponent } from './navbar/navbar.component';
import { AuthService } from './auth/auth.service';
import { PasteListComponent } from './pastes/paste-list/paste-list.component';
import { PasteService } from './pastes/paste.service';
import { WindowRefService } from './shared/window-ref.service';
import { PasteCreateComponent } from './pastes/paste-create/paste-create.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ToggleSwitchComponent } from './shared/toggle-switch/toggle-switch.component';
import { SettingsService } from './shared/settings/settings.service';
import { FileValidator } from './pastes/file.validator';
import { ClipboardDirective } from './shared/clipboard/clipboard.directive';
import { LoginComponent } from './user/login/login.component';
import { SetupAccountComponent } from './user/setup-account/setup-account.component';
import { SetupAccountService } from './user/setup-account/setup-account.service';
import { AuthGuardService } from './auth/auth-guard.service';
import { PasteListResolver } from './pastes/paste-list/paste-list.resolver';
import { PasteElementComponent } from './pastes/paste-element/paste-element.component';
import { PasteDetailComponent } from './pastes/paste-detail/paste-detail.component';
import { PasteResolver } from './pastes/paste-detail/paste.resolver';
import { NgProgressModule } from '@ngx-progressbar/core';
import { NgProgressHttpClientModule } from '@ngx-progressbar/http-client';
import { UploadLinkComponent } from './upload-links/upload-link/upload-link.component';
import { QuicklinkInputComponent } from './shared/quicklink-input/quicklink-input.component';
import { UploadLinkService } from './upload-links/upload-link/upload-link.service';
import { PasteFromLinkComponent } from './pastes/paste-from-link/paste-from-link.component';
import { UploadLinkResolver } from './pastes/paste-from-link/upload-link.resolver';
import { WelcomeComponent } from './welcome/welcome.component';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    PasteListComponent,
    PasteCreateComponent,
    ToggleSwitchComponent,
    ClipboardDirective,
    LoginComponent,
    SetupAccountComponent,
    PasteElementComponent,
    PasteDetailComponent,
    UploadLinkComponent,
    QuicklinkInputComponent,
    PasteFromLinkComponent,
    WelcomeComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    
    // modules for toastr
    BrowserAnimationsModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-center',
      timeOut: 3500
    }),

    // modules for progress bar
    NgProgressModule.forRoot(),
    NgProgressHttpClientModule,

    BsModalModule
  ],
  providers: [
    AuthService,
    AuthGuardService,
    PasteService,
    WindowRefService,
    SettingsService,
    FileValidator,
    SetupAccountService,
    PasteListResolver,
    PasteResolver,
    UploadLinkService,
    UploadLinkResolver
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
