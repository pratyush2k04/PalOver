import {
  Directive,
  ElementRef,
  HostListener,
  inject,
  input,
  OnDestroy,
  Renderer2
} from '@angular/core';
import { DOCUMENT } from '@angular/common';

export type TooltipPosition = 'top' | 'bottom' | 'left' | 'right';

@Directive({
  selector: '[appTooltip]',
  standalone: true
})
export class TooltipDirective implements OnDestroy {
  private elementRef = inject(ElementRef);
  private renderer = inject(Renderer2);
  private document = inject(DOCUMENT);

  readonly tooltipText = input<string | null>(null, { alias: 'appTooltip' });
  readonly position = input<TooltipPosition>('top', { alias: 'tooltipPosition' });
  readonly delay = input<number>(120, { alias: 'tooltipDelay' });

  private tooltipElement: HTMLElement | null = null;
  private showTimeout: ReturnType<typeof setTimeout> | null = null;
  private hideTimeout: ReturnType<typeof setTimeout> | null = null;

  @HostListener('mouseenter')
  @HostListener('focusin')
  onMouseEnter(): void {
    this.scheduleShow();
  }

  @HostListener('mouseleave')
  @HostListener('focusout')
  onMouseLeave(): void {
    this.hide();
  }

  @HostListener('click')
  onClick(): void {
    this.hide(true);
  }

  private scrollListenerAttached = false;

  private onScrollCaptured = () => {
    this.hide(true);
  };

  private attachScrollListener(): void {
    if (!this.scrollListenerAttached) {
      window.addEventListener('scroll', this.onScrollCaptured, true);
      this.scrollListenerAttached = true;
    }
  }

  private detachScrollListener(): void {
    if (this.scrollListenerAttached) {
      window.removeEventListener('scroll', this.onScrollCaptured, true);
      this.scrollListenerAttached = false;
    }
  }

  private scheduleShow(): void {
    this.clearTimers();
    const text = this.tooltipText();
    if (!text || text.trim() === '') {
      return;
    }

    this.attachScrollListener();

    this.showTimeout = setTimeout(() => {
      this.createAndPositionTooltip();
    }, this.delay());
  }

  private createAndPositionTooltip(): void {
    if (this.tooltipElement) {
      this.removeTooltipElement();
    }

    const text = this.tooltipText();
    if (!text) return;

    const hostEl = this.elementRef.nativeElement as HTMLElement;
    if (!hostEl || !hostEl.isConnected) return;

    const tooltip = this.renderer.createElement('div') as HTMLElement;
    this.renderer.addClass(tooltip, 'app-tooltip');
    this.renderer.setAttribute(tooltip, 'role', 'tooltip');

    const content = this.renderer.createElement('span');
    this.renderer.setProperty(content, 'textContent', text);
    this.renderer.appendChild(tooltip, content);

    const arrow = this.renderer.createElement('span');
    this.renderer.addClass(arrow, 'app-tooltip__arrow');
    this.renderer.appendChild(tooltip, arrow);

    this.renderer.appendChild(this.document.body, tooltip);
    this.tooltipElement = tooltip;

    this.updatePosition(hostEl, tooltip, arrow);

    requestAnimationFrame(() => {
      if (this.tooltipElement) {
        this.renderer.addClass(this.tooltipElement, 'app-tooltip--visible');
      }
    });
  }

  private updatePosition(hostEl: HTMLElement, tooltip: HTMLElement, arrow: HTMLElement): void {
    const hostRect = hostEl.getBoundingClientRect();
    const tooltipRect = tooltip.getBoundingClientRect();
    const gap = 6;
    let pos = this.position();

    let top = 0;
    let left = 0;

    if (pos === 'top' && hostRect.top - tooltipRect.height - gap < 4) {
      pos = 'bottom';
    } else if (pos === 'bottom' && hostRect.bottom + tooltipRect.height + gap > window.innerHeight - 4) {
      pos = 'top';
    }

    switch (pos) {
      case 'top':
        top = hostRect.top - tooltipRect.height - gap;
        left = hostRect.left + (hostRect.width - tooltipRect.width) / 2;
        break;
      case 'bottom':
        top = hostRect.bottom + gap;
        left = hostRect.left + (hostRect.width - tooltipRect.width) / 2;
        break;
      case 'left':
        top = hostRect.top + (hostRect.height - tooltipRect.height) / 2;
        left = hostRect.left - tooltipRect.width - gap;
        break;
      case 'right':
        top = hostRect.top + (hostRect.height - tooltipRect.height) / 2;
        left = hostRect.right + gap;
        break;
    }

    const minLeft = 6;
    const maxLeft = window.innerWidth - tooltipRect.width - 6;
    left = Math.max(minLeft, Math.min(left, maxLeft));

    this.renderer.addClass(arrow, `app-tooltip__arrow--${pos}`);

    this.renderer.setStyle(tooltip, 'top', `${Math.round(top)}px`);
    this.renderer.setStyle(tooltip, 'left', `${Math.round(left)}px`);
  }

  hide(immediate = false): void {
    this.detachScrollListener();
    this.clearTimers();
    if (!this.tooltipElement) return;

    if (immediate) {
      this.removeTooltipElement();
    } else {
      this.renderer.removeClass(this.tooltipElement, 'app-tooltip--visible');
      const el = this.tooltipElement;
      this.tooltipElement = null;
      this.hideTimeout = setTimeout(() => {
        if (el.parentNode) {
          el.parentNode.removeChild(el);
        }
      }, 150);
    }
  }

  private removeTooltipElement(): void {
    if (this.tooltipElement) {
      if (this.tooltipElement.parentNode) {
        this.tooltipElement.parentNode.removeChild(this.tooltipElement);
      }
      this.tooltipElement = null;
    }
  }

  private clearTimers(): void {
    if (this.showTimeout) {
      clearTimeout(this.showTimeout);
      this.showTimeout = null;
    }
    if (this.hideTimeout) {
      clearTimeout(this.hideTimeout);
      this.hideTimeout = null;
    }
  }

  ngOnDestroy(): void {
    this.detachScrollListener();
    this.clearTimers();
    this.removeTooltipElement();
  }
}
