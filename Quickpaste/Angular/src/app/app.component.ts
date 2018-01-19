import { Component } from '@angular/core';
import { OnInit } from '@angular/core/src/metadata/lifecycle_hooks';
import { SettingsService } from './shared/settings/settings.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  
  title = 'Quickpaste';

  constructor(private settingsService: SettingsService){}
  ngOnInit(): void {
    this.settingsService.refreshSettings();
  }

}
