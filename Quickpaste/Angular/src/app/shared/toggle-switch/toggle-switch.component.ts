import { Component, OnInit, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor } from '@angular/forms/src/directives/control_value_accessor';
import { NG_VALUE_ACCESSOR } from '@angular/forms';

/**
 * A toggle switch component which can be used as a form control by implementing ControlValueAccessor
 * 
 * @export
 * @class ToggleSwitchComponent
 * @implements {OnInit}
 * @implements {ControlValueAccessor}
 */
@Component({
  selector: 'toggle-switch',
  templateUrl: './toggle-switch.component.html',
  styleUrls: ['./toggle-switch.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ToggleSwitchComponent),
      multi: true
    }
  ]
})
export class ToggleSwitchComponent implements OnInit, ControlValueAccessor {

  // default value when component is created
  @Input() isOn: boolean = true;

  // text to display for the "true"/"on" value
  @Input() onText: string = "On";
  // text to display for the "false"/"off" value  
  @Input() offText: string = "Off";
  
  onChange: any = () => { };
  writeValue(switchState: boolean): void {
    this.isOn = switchState;
    this.onChange(this.isOn);
    
  }
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
  }

  setDisabledState?(isDisabled: boolean): void {
  }

  switchClicked(switchState: boolean) {
    this.writeValue(switchState);
    
  }

  constructor() { }

  ngOnInit() {
  }

}
