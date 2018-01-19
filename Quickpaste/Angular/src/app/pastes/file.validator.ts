import { AbstractControl } from '@angular/forms';
import { SettingsModel } from '../shared/settings/settings.model';
import { SettingsService } from '../shared/settings/settings.service';
import { Injectable } from '@angular/core';


@Injectable()
export class FileValidator {
  constructor(private settingsService: SettingsService) { }

  createValidateFile() {
    return (fileControl: AbstractControl) => {
      let settings: SettingsModel = this.settingsService.getSettings();
      if (fileControl.value === null || fileControl.value.length === 0 || fileControl.value[0].size < settings.max_file_bytes) {
        return null;
      }
      return { validateFile: `File cannot be larger than ${this.settingsService.getSettings().max_file_bytes / 1000000} MB` };
    }
  }
}