import { booleanAttribute, ChangeDetectionStrategy, Component, computed, effect, ElementRef, inject, Input, input, model, OnDestroy, signal, untracked, viewChild } from '@angular/core';
import { AbstractControl, ControlValueAccessor, NgControl } from '@angular/forms';
import { MAT_FORM_FIELD, MatFormFieldControl } from '@angular/material/form-field';
import { Subject } from 'rxjs';
import { FocusMonitor } from '@angular/cdk/a11y';
import { SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-file-upload-input',
  templateUrl: './file-upload-input.component.html',
  styleUrl: './file-upload-input.component.scss',
  standalone: false,
  providers: [{provide: MatFormFieldControl, useExisting: FileUploadInputCompontent}],
  host: {
    '[class.example-floating]': 'shouldLabelFloat',
    '[id]': 'id',
  },
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FileUploadInputCompontent implements ControlValueAccessor, MatFormFieldControl<File[]>, OnDestroy {
  @Input() multiple: boolean = false;

  static nextId = 0;
  readonly fileInput = viewChild.required<HTMLInputElement>('file');
  ngControl = inject(NgControl, {optional: true, self: true});

  readonly stateChanges = new Subject<void>();
  readonly touched = signal(false);
  readonly controlType = 'file-upload-input';
  readonly id = `file-upload-input-${FileUploadInputCompontent.nextId++}`;
  readonly _userAriaDescribedBy = input<string>('', {alias: 'aria-describedby'});
  readonly _placeholder = input<string>('', {alias: 'placeholder'});
  readonly _required = input<boolean, unknown>(false, {
    alias: 'required',
    transform: booleanAttribute,
  });
  readonly _disabledByInput = input<boolean, unknown>(false, {
    alias: 'disabled',
    transform: booleanAttribute,
  });
  readonly _value = model<File[]>([], {alias: 'value'});

  filePreviewUrls: SafeUrl[] = [];

  onChange = (_: any) => {};
  onTouched = () => {};

  protected readonly _formField = inject(MAT_FORM_FIELD, {
    optional: true,
  });

  private readonly _focused = signal(false);
  private readonly _disabledByCva = signal(false);
  private readonly _disabled = computed(() => this._disabledByInput() || this._disabledByCva());
  private readonly _focusMonitor = inject(FocusMonitor);
  private readonly _elementRef = inject<ElementRef<HTMLElement>>(ElementRef);

  get focused(): boolean {
    return this._focused();
  }

  get empty() {
    return !this.value;
  }

  shouldLabelFloat = true;

  get userAriaDescribedBy() {
    return this._userAriaDescribedBy();
  }

  get placeholder(): string {
    return this._placeholder();
  }

  get required(): boolean {
    return this._required();
  }

  get disabled(): boolean {
    return this._disabled();
  }

  get value(): File[] {
    return this._value();
  }

  get errorState(): boolean {
    return this.required && !this.value && this.touched();
  }

  constructor() {
    if (this.ngControl != null) {
      this.ngControl.valueAccessor = this;
    }

    effect(() => {
      // Read signals to trigger effect.
      this._placeholder();
      this._required();
      this._disabled();
      this._focused();
      // Propagate state changes.
      untracked(() => this.stateChanges.next());
    });
  }

  ngOnDestroy() {
    this.stateChanges.complete();
    this._focusMonitor.stopMonitoring(this._elementRef);
  }

  onFocusIn() {
    if (!this._focused()) {
      this._focused.set(true);
    }
  }

  onFocusOut(event: FocusEvent) {
    if (!this._elementRef.nativeElement.contains(event.relatedTarget as Element)) {
      this.touched.set(true);
      this._focused.set(false);
      this.onTouched();
    }
  }

  autoFocusNext(control: AbstractControl, nextElement?: HTMLInputElement): void {
    if (!control.errors && nextElement) {
      this._focusMonitor.focusVia(nextElement, 'program');
    }
  }

  autoFocusPrev(control: AbstractControl, prevElement: HTMLInputElement): void {
    if (control.value.length < 1) {
      this._focusMonitor.focusVia(prevElement, 'program');
    }
  }

  setDescribedByIds(ids: string[]) {
    const controlElement = this._elementRef.nativeElement.querySelector(
      '.file-upload-input-container',
    )!;
    controlElement.setAttribute('aria-describedby', ids.join(' '));
  }

  onContainerClick() {
    this._focusMonitor.focusVia(this.fileInput(), 'program');
  }

  writeValue(file: File[] | null): void {
    if (file === null)
      file = [];

    this._updateValue(file);
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this._disabledByCva.set(isDisabled);
  }

  onFileSelected(event: any): void {
    const value = [...event.target.files];

    this._updateValue(value);
    this.onChange(value);
  }

  private _updateValue(files: File[]) {
    const current = this._value();

    if (files === current) {
      return;
    }

    this._value.set(files);

    if (files.length) {
      this.filePreviewUrls = files.map(file => URL.createObjectURL(file));
    }
  }
}

