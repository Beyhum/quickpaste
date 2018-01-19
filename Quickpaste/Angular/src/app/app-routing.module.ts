import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PasteListComponent } from './pastes/paste-list/paste-list.component';
import { PasteCreateComponent } from './pastes/paste-create/paste-create.component';
import { LoginComponent } from './user/login/login.component';
import { SetupAccountComponent } from './user/setup-account/setup-account.component';
import { AuthGuardService } from './auth/auth-guard.service';
import { PasteListResolver } from './pastes/paste-list/paste-list.resolver';
import { PasteDetailComponent } from './pastes/paste-detail/paste-detail.component';
import { PasteResolver } from './pastes/paste-detail/paste.resolver';
import { UploadLinkComponent } from './upload-links/upload-link/upload-link.component';
import { UploadLinkResolver } from './pastes/paste-from-link/upload-link.resolver';
import { PasteFromLinkComponent } from './pastes/paste-from-link/paste-from-link.component';
import { WelcomeComponent } from './welcome/welcome.component';

const routes: Routes = [

  {path: 'pastes', component: PasteListComponent, canActivate: [AuthGuardService], resolve: {pasteList: PasteListResolver} },
  {path: 'pastes/new', component: PasteCreateComponent, canActivate: [AuthGuardService]},  
  {path: 'u', component: UploadLinkComponent, canActivate: [AuthGuardService]},  
  {path: 'pastes/:quick_link', component: PasteDetailComponent, resolve: {paste: PasteResolver}},
  {path: 'u/:quick_link', component: PasteFromLinkComponent, resolve: {uploadLink: UploadLinkResolver}},
  {path: 'login', component: LoginComponent},
  {path: 'setup', component: SetupAccountComponent},
  {path: 'welcome', component: WelcomeComponent},
  {path: '', redirectTo: '/pastes' , pathMatch: 'full'}  
  
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
