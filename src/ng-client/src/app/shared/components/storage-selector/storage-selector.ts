import {
  Component,
  DestroyRef,
  EventEmitter,
  Input,
  OnInit,
  Optional,
  Output,
  Self,
  inject,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, debounceTime, distinctUntilChanged, finalize, of, startWith, switchMap, tap } from 'rxjs';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SelectListItem } from '../../../core/models/common.model';
import { StorageService } from '../../../features/storage/storage.service';

@Component({
  selector: 'app-storage-selector',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatAutocompleteModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './storage-selector.html',
  styleUrl: './storage-selector.scss',
  providers: [StorageService],
})
export class StorageSelector implements OnInit, ControlValueAccessor {
  @Input() label = 'Storage';
  @Input() placeholder = 'Search storage';
  @Input() required = false;
  @Input() allowClear = true;

  @Output() selectedItemChange = new EventEmitter<SelectListItem | null>();

  private readonly destroyRef = inject(DestroyRef);

  public readonly items = signal<SelectListItem[]>([]);
  public readonly loading = signal(false);
  public readonly searchControl = new FormControl<string | SelectListItem>('', { nonNullable: true });
  public readonly selectedItem = signal<SelectListItem | null>(null);
  public readonly disabled = signal(false);

  private onChange: (value: string | null) => void = () => {};
  private onTouched: () => void = () => {};

  constructor(
    private storageService: StorageService,
    @Optional() @Self() public ngControl: NgControl | null
  ) {
    if (this.ngControl) {
      this.ngControl.valueAccessor = this;
    }
  }

  ngOnInit() {
    this.searchControl.valueChanges
      .pipe(
        startWith(this.searchControl.value),
        debounceTime(250),
        distinctUntilChanged(),
        tap((value) => {
          if (typeof value === 'string') {
            const selected = this.selectedItem();
            if (selected && value !== selected.text) {
              this.selectedItem.set(null);
              this.onChange(null);
              this.selectedItemChange.emit(null);
            }
          }
          this.loading.set(true);
        }),
        switchMap((value) => {
          const query = typeof value === 'string' ? value : value?.text ?? '';
          return this.storageService.listItems(query).pipe(
            catchError((error) => {
              console.error('Error loading storage select list items:', error);
              return of([] as SelectListItem[]);
            }),
            finalize(() => this.loading.set(false))
          );
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe((result) => {
        this.items.set(result);
      });
  }

  // ---- ControlValueAccessor ----

  writeValue(value: string | null): void {
    if (!value) {
      this.selectedItem.set(null);
      this.searchControl.setValue('', { emitEvent: false });
      return;
    }

    const current = this.selectedItem();
    if (current?.value === value) {
      return;
    }

    this.loading.set(true);
    this.storageService
      .getById(value)
      .pipe(
        catchError((error) => {
          console.error('Error loading storage by id:', error);
          return of(null);
        }),
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe((storage) => {
        if (!storage) return;
        const item: SelectListItem = { value: storage.id, text: storage.name };
        this.selectedItem.set(item);
        this.searchControl.setValue(item, { emitEvent: false });
      });
  }

  registerOnChange(fn: (value: string | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
    if (isDisabled) {
      this.searchControl.disable({ emitEvent: false });
    } else {
      this.searchControl.enable({ emitEvent: false });
    }
  }

  // ---- UI handlers ----

  onOptionSelected(item: SelectListItem) {
    this.selectedItem.set(item);
    this.searchControl.setValue(item, { emitEvent: false });
    this.onChange(item.value);
    this.onTouched();
    this.selectedItemChange.emit(item);
  }

  clearSelection() {
    this.selectedItem.set(null);
    this.searchControl.setValue('', { emitEvent: true });
    this.onChange(null);
    this.onTouched();
    this.selectedItemChange.emit(null);
  }

  displayWith = (value: SelectListItem | string | null): string => {
    if (!value) return '';
    return typeof value === 'string' ? value : value.text;
  };

  markTouched() {
    this.onTouched();
  }
}
