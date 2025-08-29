/**
 * Form Control Binder - Tự động bind các data attributes thành form controls
 */
class FormControlBinder {
    constructor() {
        // Bind methods to preserve 'this' context
        this.controlTypes = {
            'Text': this.createTextInput.bind(this),
            'Email': this.createEmailInput.bind(this),
            'Password': this.createPasswordInput.bind(this),
            'Number': this.createNumberInput.bind(this),
            'Tel': this.createTelInput.bind(this),
            'Url': this.createUrlInput.bind(this),
            'Textarea': this.createTextarea.bind(this),
            'Select': this.createSelect.bind(this),
            'MultiSelect': this.createMultiSelect.bind(this),
            'Radio': this.createRadio.bind(this),
            'Checkbox': this.createCheckbox.bind(this),
            'CheckboxGroup': this.createCheckboxGroup.bind(this),
            'Date': this.createDateInput.bind(this),
            'DateTime': this.createDateTimeInput.bind(this),
            'Time': this.createTimeInput.bind(this),
            'File': this.createFileInput.bind(this),
            'Image': this.createImageInput.bind(this),
            'Color': this.createColorInput.bind(this),
            'Range': this.createRangeInput.bind(this),
            'Switch': this.createSwitch.bind(this),
            'Toggle': this.createToggle.bind(this),
            'Hidden': this.createHiddenInput.bind(this),
            'Display': this.createDisplay.bind(this),
            'ModalHeader': this.createModalHeader.bind(this),
            'ModalFooter': this.createModalFooter.bind(this),
            'Form': this.createForm.bind(this)
        };
    }

    /**
     * Khởi tạo và bind tất cả controls trong container
     * @param {string|HTMLElement} container - Container selector hoặc element
     */
    init(container = document) {
        const $container = typeof container === 'string'
            ? $(container)
            : $(container);

        if ($container.length === 0) {
            console.warn('FormControlBinder: Container not found');
            return;
        }

        // Tìm tất cả elements có data-type
        const $elements = $container.find('[data-type]');

        $elements.each((index, element) => {
            this.bindControl(element);
        });

        // Setup validation cho form nếu container là form
        if ($container[0] && $container[0].tagName === 'FORM') {
            this.setupFormValidation($container[0]);
        }

        // Setup button handlers cho các form được tạo bởi ControlType.Form
        this.setupButtonHandlers($container[0]);
    }

    /**
     * Setup validation cho form
     * @param {HTMLElement} form - Form element
     */
    setupFormValidation(form) {
        // Add novalidate để tắt browser validation
        form.setAttribute('novalidate', 'novalidate');

        // Add submit event listener using jQuery
        const $form = $(form);
        $form.on('submit', (e) => {
            e.preventDefault();
            if (this.validateForm(form)) {
                // Auto submit nếu có data-url
                const submitUrl = $form.attr('data-url');
                if (submitUrl) {
                    this.autoSubmitForm(form);
                } else {
                    // Trigger custom event nếu validation pass
                    $form.trigger('formValidated', {
                        isValid: true,
                        formData: this.getFormData(form)
                    });
                }
            }
        });

        // Add real-time validation using jQuery
        $form.on('input', 'input, select, textarea', (e) => {
            this.validateField(e.target);
        });

        $form.on('change', 'input, select, textarea', (e) => {
            this.validateField(e.target);
        });
    }

    /**
     * Bind một control element
     * @param {HTMLElement} element - Element cần bind
     */
    bindControl(element) {
        const $element = $(element);
        const type = $element.attr('data-type');
        const label = $element.attr('data-label') || '';
        const value = $element.attr('data-value') || '';
        const name = $element.attr('data-name') || '';
        const id = $element.attr('data-id') || this.generateId(name);
        const required = $element.is('[data-required]');
        const disabled = $element.is('[data-disabled]');
        const readonly = $element.is('[data-readonly]');
        const placeholder = $element.attr('data-placeholder') || '';
        const cssClass = $element.attr('data-css-class') || '';
        const customClass = $element.attr('data-class') || '';
        const wrapperClass = $element.attr('data-wrapper-class') || '';
        const helpText = $element.attr('data-help-text') || '';
        const options = $element.attr('data-options') || '';

        // Parse options nếu có
        let optionsList = [];
        if (options) {
            try {
                optionsList = JSON.parse(options);
            } catch (e) {
                console.warn('FormControlBinder: Invalid options JSON', options);
            }
        }

        const config = {
            type,
            label,
            value,
            name,
            id,
            required,
            disabled,
            readonly,
            placeholder,
            cssClass,
            customClass,
            wrapperClass,
            helpText,
            options: optionsList,
            element
        };

        // Tìm handler cho control type
        const handler = this.controlTypes[type];

        if (handler) {
            try {
                const html = handler(config);

                if (!html || html.trim() === '') {
                    return;
                }

                // Create a temporary container to parse HTML
                const tempDiv = document.createElement('div');
                tempDiv.innerHTML = html;
                const newElement = tempDiv.firstElementChild;

                if (newElement && $element.parent().length > 0) {
                    // Use jQuery to replace element
                    $element.replaceWith($(newElement));
                }
            } catch (error) {
                console.error(`Error calling handler for type ${type}:`, error);
            }
        }
    }

    /**
     * Tạo unique ID
     */
    generateId(name) {
        return name ? name.replace(/[\[\]]/g, '_') + '_' + Date.now() : 'control_' + Date.now();
    }

    /**
     * Tạo attributes string
     */
    createAttributes(config) {
        const attrs = [];

        if (config.required) attrs.push('required');
        if (config.disabled) attrs.push('disabled');
        if (config.readonly) attrs.push('readonly');
        if (config.placeholder) attrs.push(`placeholder="${config.placeholder}"`);

        return attrs.join(' ');
    }

    /**
     * Combine CSS classes
     */
    combineClasses(baseClass, customClass) {
        let classes = baseClass;
        if (customClass) {
            classes += ' ' + customClass;
        }
        return classes;
    }

    /**
     * Text Input
     */
    createTextInput(config) {
        const inputClass = this.combineClasses('form-control', config.customClass);
        const wrapperClass = this.combineClasses('mb-3', config.wrapperClass);
        return `
            <div class="${wrapperClass}">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <input type="text" class="${inputClass}" id="${config.id}" name="${config.name}" value="${config.value}" ${this.createAttributes(config)}>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Email Input
     */
    createEmailInput(config) {
        const inputClass = this.combineClasses('form-control', config.customClass);
        const wrapperClass = this.combineClasses('mb-3', config.wrapperClass);
        return `
            <div class="${wrapperClass}">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <input type="email" class="${inputClass}" id="${config.id}" name="${config.name}" value="${config.value}" ${this.createAttributes(config)}>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Password Input
     */
    createPasswordInput(config) {
        const inputClass = this.combineClasses('form-control', config.customClass);
        return `
            <div class="mb-3">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <input type="password" class="${inputClass}" id="${config.id}" name="${config.name}" value="${config.value}" ${this.createAttributes(config)}>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Number Input
     */
    createNumberInput(config) {
        const min = config.element.getAttribute('data-min') || '';
        const max = config.element.getAttribute('data-max') || '';
        const step = config.element.getAttribute('data-step') || '';
        const inputClass = this.combineClasses('form-control', config.customClass);

        return `
            <div class="mb-3">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <input type="number" class="${inputClass}" id="${config.id}" name="${config.name}" value="${config.value}"
                       ${min ? `min="${min}"` : ''} ${max ? `max="${max}"` : ''} ${step ? `step="${step}"` : ''} ${this.createAttributes(config)}>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Tel Input
     */
    createTelInput(config) {
        const inputClass = this.combineClasses('form-control', config.customClass);
        return `
            <div class="mb-3">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <input type="tel" class="${inputClass}" id="${config.id}" name="${config.name}" value="${config.value}" ${this.createAttributes(config)}>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * URL Input
     */
    createUrlInput(config) {
        const inputClass = this.combineClasses('form-control', config.customClass);
        return `
            <div class="mb-3">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <input type="url" class="${inputClass}" id="${config.id}" name="${config.name}" value="${config.value}" ${this.createAttributes(config)}>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Textarea
     */
    createTextarea(config) {
        const rows = config.element.getAttribute('data-rows') || '3';
        const inputClass = this.combineClasses('form-control', config.customClass);

        return `
            <div class="mb-3">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <textarea class="${inputClass}" id="${config.id}" name="${config.name}" rows="${rows}" ${this.createAttributes(config)}>${config.value}</textarea>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Select
     */
    createSelect(config) {
        let optionsHtml = '';
        if (config.options && config.options.length > 0) {
            optionsHtml = config.options.map(option =>
                `<option value="${option.value}" ${option.selected || option.value === config.value ? 'selected' : ''}>${option.text}</option>`
            ).join('');
        }

        const selectClass = this.combineClasses('form-select', config.customClass);
        const wrapperClass = this.combineClasses('mb-3', config.wrapperClass);
        return `
            <div class="${wrapperClass}">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <select class="${selectClass}" id="${config.id}" name="${config.name}" ${this.createAttributes(config)}>
                    ${optionsHtml}
                </select>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Multi Select
     */
    createMultiSelect(config) {
        let optionsHtml = '';
        if (config.options && config.options.length > 0) {
            optionsHtml = config.options.map(option => 
                `<option value="${option.value}" ${option.selected ? 'selected' : ''}>${option.text}</option>`
            ).join('');
        }

        return `
            <div class="mb-3">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <select class="form-select ${config.cssClass}" id="${config.id}" name="${config.name}" multiple ${this.createAttributes(config)}>
                    ${optionsHtml}
                </select>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Checkbox
     */
    createCheckbox(config) {
        const checked = config.value === 'true' || config.element.hasAttribute('data-checked') ? 'checked' : '';
        const inputClass = this.combineClasses('form-check-input', config.customClass);

        return `
            <div class="form-check">
                <input class="${inputClass}" type="checkbox" id="${config.id}" name="${config.name}" value="${config.value}" ${checked} ${this.createAttributes(config)}>
                <label class="form-check-label" for="${config.id}">
                    ${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}
                </label>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Toggle/Switch (giống checkbox nhưng style khác)
     */
    createToggle(config) {
        const checked = config.value === 'true' || config.element.hasAttribute('data-checked') ? 'checked' : '';
        const inputClass = this.combineClasses('form-check-input', config.customClass);

        return `
            <div class="form-check form-switch">
                <input class="${inputClass}" type="checkbox" id="${config.id}" name="${config.name}" value="${config.value}" ${checked} ${this.createAttributes(config)}>
                <label class="form-check-label" for="${config.id}">
                    ${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}
                </label>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Switch (alias cho Toggle)
     */
    createSwitch(config) {
        return this.createToggle(config);
    }

    /**
     * Radio
     */
    createRadio(config) {
        let radioHtml = '';
        if (config.options && config.options.length > 0) {
            radioHtml = config.options.map((option, index) => {
                const radioId = `${config.id}_${index}`;
                const checked = option.selected || option.value === config.value ? 'checked' : '';
                return `
                    <div class="form-check">
                        <input class="form-check-input ${config.cssClass}" type="radio" id="${radioId}" name="${config.name}" value="${option.value}" ${checked} ${this.createAttributes(config)}>
                        <label class="form-check-label" for="${radioId}">
                            ${option.text}
                        </label>
                    </div>
                `;
            }).join('');
        }

        return `
            <div class="mb-3">
                ${config.label ? `<label class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                ${radioHtml}
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Checkbox Group
     */
    createCheckboxGroup(config) {
        let checkboxHtml = '';
        if (config.options && config.options.length > 0) {
            checkboxHtml = config.options.map((option, index) => {
                const checkboxId = `${config.id}_${index}`;
                const checked = option.selected ? 'checked' : '';
                return `
                    <div class="form-check">
                        <input class="form-check-input ${config.cssClass}" type="checkbox" id="${checkboxId}" name="${config.name}" value="${option.value}" ${checked} ${this.createAttributes(config)}>
                        <label class="form-check-label" for="${checkboxId}">
                            ${option.text}
                        </label>
                    </div>
                `;
            }).join('');
        }

        return `
            <div class="mb-3">
                ${config.label ? `<label class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                ${checkboxHtml}
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Date Input
     */
    createDateInput(config) {
        return `
            <div class="mb-3">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <input type="date" class="form-control ${config.cssClass}" id="${config.id}" name="${config.name}" value="${config.value}" ${this.createAttributes(config)}>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * DateTime Input
     */
    createDateTimeInput(config) {
        return `
            <div class="mb-3">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <input type="datetime-local" class="form-control ${config.cssClass}" id="${config.id}" name="${config.name}" value="${config.value}" ${this.createAttributes(config)}>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Time Input
     */
    createTimeInput(config) {
        return `
            <div class="mb-3">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <input type="time" class="form-control ${config.cssClass}" id="${config.id}" name="${config.name}" value="${config.value}" ${this.createAttributes(config)}>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * File Input
     */
    createFileInput(config) {
        const accept = config.element.getAttribute('data-accept') || '';
        const multiple = config.element.hasAttribute('data-multiple') ? 'multiple' : '';
        
        return `
            <div class="mb-3">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <input type="file" class="form-control ${config.cssClass}" id="${config.id}" name="${config.name}" 
                       ${accept ? `accept="${accept}"` : ''} ${multiple} ${this.createAttributes(config)}>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Image Input
     */
    createImageInput(config) {
        return `
            <div class="mb-3">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <input type="file" class="form-control ${config.cssClass}" id="${config.id}" name="${config.name}" accept="image/*" ${this.createAttributes(config)}>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Color Input
     */
    createColorInput(config) {
        return `
            <div class="mb-3">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <input type="color" class="form-control form-control-color ${config.cssClass}" id="${config.id}" name="${config.name}" value="${config.value}" ${this.createAttributes(config)}>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Range Input
     */
    createRangeInput(config) {
        const min = config.element.getAttribute('data-min') || '0';
        const max = config.element.getAttribute('data-max') || '100';
        const step = config.element.getAttribute('data-step') || '1';
        
        return `
            <div class="mb-3">
                ${config.label ? `<label for="${config.id}" class="form-label">${config.label}${config.required ? ' <span class="text-danger">*</span>' : ''}</label>` : ''}
                <input type="range" class="form-range ${config.cssClass}" id="${config.id}" name="${config.name}" value="${config.value}" 
                       min="${min}" max="${max}" step="${step}" ${this.createAttributes(config)}>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Hidden Input
     */
    createHiddenInput(config) {
        return `<input type="hidden" id="${config.id}" name="${config.name}" value="${config.value}">`;
    }

    /**
     * Display (read-only text)
     */
    createDisplay(config) {
        return `
            <div class="mb-3">
                ${config.label ? `<label class="form-label">${config.label}</label>` : ''}
                <div class="form-control-plaintext">${config.value}</div>
                ${config.helpText ? `<div class="form-text">${config.helpText}</div>` : ''}
            </div>
        `;
    }

    /**
     * Modal Header
     */
    createModalHeader(config) {
        const title = config.label || config.value || 'Modal Title';
        const showCloseButton = !config.element.hasAttribute('data-no-close');
        const headerClass = this.combineClasses('modal-header', config.customClass);

        return `
            <div class="${headerClass}">
                <h5 class="modal-title" id="${config.id}">${title}</h5>
                ${showCloseButton ? `
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                ` : ''}
            </div>
        `;
    }

    /**
     * Modal Footer
     */
    createModalFooter(config) {
        const footerClass = this.combineClasses('modal-footer', config.customClass);

        // Parse buttons from data-buttons attribute
        let buttonsHtml = '';
        const buttonsData = config.element.getAttribute('data-buttons');

        if (buttonsData) {
            try {
                const buttons = JSON.parse(buttonsData);
                buttonsHtml = buttons.map(button => {
                    const btnClass = button.class || 'btn btn-secondary';
                    const btnType = button.type || 'button';
                    const btnId = button.id || '';
                    const btnAction = button.action || '';
                    const btnIcon = button.icon ? `<i class="${button.icon} me-2"></i>` : '';

                    return `
                        <button type="${btnType}"
                                class="${btnClass}"
                                ${btnId ? `id="${btnId}"` : ''}
                                ${btnAction ? `data-action="${btnAction}"` : ''}
                                ${button.dismiss ? 'data-bs-dismiss="modal"' : ''}>
                            ${btnIcon}${button.text}
                        </button>
                    `;
                }).join('');
            } catch (e) {
                console.warn('FormControlBinder: Invalid buttons JSON', buttonsData);
            }
        }

        // Default buttons nếu không có data-buttons
        if (!buttonsHtml) {
            buttonsHtml = `
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                    <i class="bi bi-x-lg me-2"></i>Hủy
                </button>
                <button type="button" class="btn btn-primary">
                    <i class="bi bi-check-lg me-2"></i>Lưu
                </button>
            `;
        }

        return `
            <div class="${footerClass}">
                ${buttonsHtml}
            </div>
        `;
    }

    /**
     * Form wrapper với modal header/footer
     */
    createForm(config) {
        const title = config.label || 'Form Title';
        const saveButtonId = config.element.getAttribute('data-button-save-id') || 'saveBtn';
        const cancelButtonId = config.element.getAttribute('data-button-cancel-id') || 'cancelBtn';
        const formId = config.element.getAttribute('data-form-id') || config.id + '_form';
        const modalId = config.element.getAttribute('data-modal-id') || config.id + '_modal';

        // Lấy nội dung body từ innerHTML hiện tại
        const bodyContent = config.element.innerHTML;

        // Parse custom buttons nếu có
        const customButtons = config.element.getAttribute('data-buttons');
        let footerButtons = '';

        if (customButtons) {
            try {
                const buttons = JSON.parse(customButtons);
                footerButtons = buttons.map(button => {
                    const btnClass = button.class || 'btn btn-secondary';
                    const btnType = button.type || 'button';
                    const btnId = button.id || '';
                    const btnIcon = button.icon ? `<i class="${button.icon} me-2"></i>` : '';

                    return `
                        <button type="${btnType}"
                                class="${btnClass}"
                                ${btnId ? `id="${btnId}"` : ''}
                                ${button.dismiss ? 'data-bs-dismiss="modal"' : ''}>
                            ${btnIcon}${button.text}
                        </button>
                    `;
                }).join('');
            } catch (e) {
                console.warn('FormControlBinder: Invalid buttons JSON', customButtons);
            }
        }

        // Default buttons nếu không có custom buttons
        if (!footerButtons) {
            footerButtons = `
                <button type="button" class="btn btn-secondary" id="${cancelButtonId}" data-bs-dismiss="modal">
                    <i class="bi bi-x-lg me-2"></i>Hủy
                </button>
                <button type="button" class="btn btn-primary" id="${saveButtonId}">
                    <i class="bi bi-check-lg me-2"></i>Lưu
                </button>
            `;
        }

        // Kiểm tra xem có phải modal không
        const isModal = config.element.hasAttribute('data-modal') || config.element.closest('.modal');
        const headerClass = this.combineClasses('modal-header', config.customClass);
        const footerClass = 'modal-footer';

        if (isModal) {
            // Render as modal
            return `
                <div class="${headerClass}">
                    <h5 class="modal-title" id="${modalId}Label">${title}</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <form id="${formId}" novalidate>
                        ${bodyContent}
                    </form>
                </div>
                <div class="${footerClass}">
                    ${footerButtons}
                </div>
            `;
        } else {
            // Render as card
            const cardClass = this.combineClasses('card', config.customClass);
            return `
                <div class="${cardClass}">
                    <div class="card-header">
                        <h5 class="card-title mb-0">${title}</h5>
                    </div>
                    <div class="card-body">
                        <form id="${formId}" novalidate>
                            ${bodyContent}
                        </form>
                    </div>
                    <div class="card-footer">
                        ${footerButtons}
                    </div>
                </div>
            `;
        }
    }

    // ===== VALIDATION METHODS =====

    /**
     * Validate toàn bộ form
     * @param {HTMLElement} form - Form element
     * @returns {boolean} - True nếu form valid
     */
    validateForm(form) {
        let isValid = true;
        const $fields = $(form).find('input, select, textarea');

        $fields.each((index, field) => {
            if (!this.validateField(field)) {
                isValid = false;
            }
        });

        return isValid;
    }

    /**
     * Validate một field
     * @param {HTMLElement} field - Field element
     * @returns {boolean} - True nếu field valid
     */
    validateField(field) {
        const rules = this.getValidationRules(field);
        const value = this.getFieldValue(field);
        let isValid = true;
        let errorMessage = '';

        // Check required
        if (rules.required && this.isEmpty(value)) {
            isValid = false;
            errorMessage = rules.requiredMessage || `${this.getFieldLabel(field)} là bắt buộc`;
        }
        // Check pattern
        else if (rules.pattern && value && !rules.pattern.test(value)) {
            isValid = false;
            errorMessage = rules.patternMessage || `${this.getFieldLabel(field)} không đúng định dạng`;
        }
        // Check min/max length
        else if (rules.minLength && value && value.length < rules.minLength) {
            isValid = false;
            errorMessage = `${this.getFieldLabel(field)} phải có ít nhất ${rules.minLength} ký tự`;
        }
        else if (rules.maxLength && value && value.length > rules.maxLength) {
            isValid = false;
            errorMessage = `${this.getFieldLabel(field)} không được quá ${rules.maxLength} ký tự`;
        }
        // Check min/max value
        else if (rules.min !== undefined && value && parseFloat(value) < rules.min) {
            isValid = false;
            errorMessage = `${this.getFieldLabel(field)} phải lớn hơn hoặc bằng ${rules.min}`;
        }
        else if (rules.max !== undefined && value && parseFloat(value) > rules.max) {
            isValid = false;
            errorMessage = `${this.getFieldLabel(field)} phải nhỏ hơn hoặc bằng ${rules.max}`;
        }
        // Check email
        else if (field.type === 'email' && value && !this.isValidEmail(value)) {
            isValid = false;
            errorMessage = 'Email không đúng định dạng';
        }
        // Check URL
        else if (field.type === 'url' && value && !this.isValidUrl(value)) {
            isValid = false;
            errorMessage = 'URL không đúng định dạng';
        }

        // Apply validation state
        this.applyValidationState(field, isValid, errorMessage);

        return isValid;
    }

    /**
     * Lấy validation rules từ field
     * @param {HTMLElement} field - Field element
     * @returns {Object} - Validation rules
     */
    getValidationRules(field) {
        return {
            required: field.hasAttribute('required'),
            requiredMessage: field.getAttribute('data-required-message'),
            pattern: field.pattern ? new RegExp(field.pattern) : null,
            patternMessage: field.getAttribute('data-pattern-message'),
            minLength: field.minLength > 0 ? field.minLength : null,
            maxLength: field.maxLength > 0 ? field.maxLength : null,
            min: field.min ? parseFloat(field.min) : undefined,
            max: field.max ? parseFloat(field.max) : undefined
        };
    }

    /**
     * Lấy giá trị của field
     * @param {HTMLElement} field - Field element
     * @returns {string} - Field value
     */
    getFieldValue(field) {
        if (field.type === 'checkbox' || field.type === 'radio') {
            return field.checked ? field.value : '';
        }
        return field.value || '';
    }

    /**
     * Lấy label của field
     * @param {HTMLElement} field - Field element
     * @returns {string} - Field label
     */
    getFieldLabel(field) {
        const $label = $(field).closest('.mb-3').find('label');
        return $label.length > 0 ? $label.text().replace('*', '').trim() : 'Trường này';
    }

    /**
     * Check nếu value rỗng
     * @param {string} value - Value to check
     * @returns {boolean} - True nếu rỗng
     */
    isEmpty(value) {
        return !value || value.trim() === '';
    }

    /**
     * Validate email format
     * @param {string} email - Email to validate
     * @returns {boolean} - True nếu email hợp lệ
     */
    isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    /**
     * Validate URL format
     * @param {string} url - URL to validate
     * @returns {boolean} - True nếu URL hợp lệ
     */
    isValidUrl(url) {
        try {
            new URL(url);
            return true;
        } catch {
            return false;
        }
    }

    /**
     * Apply validation state to field
     * @param {HTMLElement} field - Field element
     * @param {boolean} isValid - Validation result
     * @param {string} errorMessage - Error message
     */
    applyValidationState(field, isValid, errorMessage) {
        const $field = $(field);

        // Remove existing validation classes
        $field.removeClass('is-valid is-invalid');

        // Remove existing error message
        $field.parent().find('.invalid-feedback').remove();

        if (isValid) {
            $field.addClass('is-valid');
        } else {
            $field.addClass('is-invalid');

            // Add error message
            const $errorDiv = $('<div class="invalid-feedback"></div>').text(errorMessage);
            $field.parent().append($errorDiv);
        }
    }

    /**
     * Lấy dữ liệu form
     * @param {HTMLElement} form - Form element
     * @returns {Object} - Form data
     */
    getFormData(form) {
        const formData = new FormData(form);
        const data = Object.fromEntries(formData.entries());

        // Handle multiple values (checkboxes, multi-select)
        const $fields = $(form).find('input, select, textarea');
        const multipleData = {};

        $fields.each((index, field) => {
            if (field.type === 'checkbox' && field.name.endsWith('[]')) {
                const name = field.name.replace('[]', '');
                if (!multipleData[name]) multipleData[name] = [];
                if (field.checked) multipleData[name].push(field.value);
            } else if (field.multiple) {
                const selectedOptions = Array.from(field.selectedOptions).map(option => option.value);
                multipleData[field.name] = selectedOptions;
            }
        });

        return { ...data, ...multipleData };
    }

    /**
     * Reset validation state của form
     * @param {HTMLElement} form - Form element
     */
    resetValidation(form) {
        const $form = $(form);
        const $fields = $form.find('input, select, textarea');

        $fields.removeClass('is-valid is-invalid');
        $form.find('.invalid-feedback').remove();
    }

    /**
     * Hiển thị validation summary
     * @param {HTMLElement} form - Form element
     * @returns {Array} - Array of validation errors
     */
    getValidationSummary(form) {
        const errors = [];
        const $fields = $(form).find('input, select, textarea');

        $fields.each((index, field) => {
            if (!this.validateField(field)) {
                const label = this.getFieldLabel(field);
                const $errorDiv = $(field).parent().find('.invalid-feedback');
                const message = $errorDiv.length > 0 ? $errorDiv.text() : 'Có lỗi xảy ra';
                errors.push({ field: field.name, label, message });
            }
        });

        return errors;
    }

    // ===== AUTO SUBMIT METHODS =====

    /**
     * Setup button handlers cho form controls
     * @param {HTMLElement} container - Container element
     */
    setupButtonHandlers(container) {
        const $container = $(container);

        // Tìm tất cả buttons có data-form-submit
        const $submitButtons = $container.find('[data-form-submit]');

        $submitButtons.on('click', (e) => {
            e.preventDefault();
            const $button = $(e.currentTarget);
            const formSelector = $button.attr('data-form-submit');
            const $form = $(formSelector);

            if ($form.length > 0) {
                this.handleFormSubmit($form[0], e.currentTarget);
            }
        });

        // Auto-setup cho các button được tạo bởi ControlType.Form
        const $formElements = $container.find('[data-type="Form"]');
        $formElements.each((index, formElement) => {
            const $formElement = $(formElement);
            const saveButtonId = $formElement.attr('data-button-save-id');
            if (saveButtonId) {
                // Delay để đảm bảo button đã được render
                setTimeout(() => {
                    const $saveButton = $('#' + saveButtonId);
                    if ($saveButton.length > 0 && !$saveButton.attr('data-form-submit-setup')) {
                        $saveButton.attr('data-form-submit-setup', 'true');
                        $saveButton.on('click', (e) => {
                            e.preventDefault();
                            const formId = $formElement.attr('data-form-id') || formElement.id + '_form';
                            const $form = $('#' + formId);

                            if ($form.length > 0) {
                                this.handleFormSubmit($form[0], $saveButton[0]);
                            }
                        });
                    }
                }, 100);
            }
        });
    }

    /**
     * Handle form submit với validation và auto-submit
     * @param {HTMLElement} form - Form element
     * @param {HTMLElement} button - Button element
     */
    handleFormSubmit(form, button) {
        // Validate form trước
        if (!this.validateForm(form)) {
            this.showNotification('❌ Vui lòng kiểm tra lại thông tin!', 'error');
            return;
        }

        // Lấy URL và method từ form hoặc button
        const submitUrl = form.getAttribute('data-url') || button.getAttribute('data-url');
        const submitMethod = form.getAttribute('data-method') || button.getAttribute('data-method') || 'POST';

        if (submitUrl) {
            this.autoSubmitForm(form, submitUrl, submitMethod, button);
        } else {
            // Trigger custom event nếu không có URL
            form.dispatchEvent(new CustomEvent('formValidated', {
                detail: { isValid: true, formData: this.getFormData(form), button }
            }));
        }
    }

    /**
     * Auto submit form với AJAX
     * @param {HTMLElement} form - Form element
     * @param {string} url - Submit URL
     * @param {string} method - HTTP method
     * @param {HTMLElement} button - Submit button
     */
    autoSubmitForm(form, url = null, method = 'POST', button = null) {
        const submitUrl = url || form.getAttribute('data-url');
        const submitMethod = method || form.getAttribute('data-method') || 'POST';

        if (!submitUrl) {
            console.warn('FormControlBinder: No submit URL found');
            return;
        }

        // Show loading state
        const originalButtonText = button ? button.innerHTML : '';
        if (button) {
            button.disabled = true;
            button.innerHTML = '<i class="spinner-border spinner-border-sm me-2"></i>Đang xử lý...';
        }

        // Get form data
        const formData = this.getFormData(form);

        // Prepare AJAX request
        const ajaxConfig = {
            url: submitUrl,
            type: submitMethod.toUpperCase(),
            data: submitMethod.toUpperCase() === 'GET' ? formData : JSON.stringify(formData),
            contentType: submitMethod.toUpperCase() === 'GET' ? 'application/x-www-form-urlencoded' : 'application/json',
            success: (response) => {
                this.handleSubmitSuccess(response, form, button, originalButtonText);
            },
            error: (xhr, status, error) => {
                this.handleSubmitError(xhr, error, form, button, originalButtonText);
            }
        };

        // Make AJAX request
        if (typeof $ !== 'undefined') {
            $.ajax(ajaxConfig);
        } else {
            // Fallback to fetch API
            this.fetchSubmit(submitUrl, submitMethod, formData, form, button, originalButtonText);
        }
    }

    /**
     * Handle submit success
     * @param {Object} response - Response data
     * @param {HTMLElement} form - Form element
     * @param {HTMLElement} button - Submit button
     * @param {string} originalButtonText - Original button text
     */
    handleSubmitSuccess(response, form, button, originalButtonText) {
        // Restore button state
        if (button) {
            button.disabled = false;
            button.innerHTML = originalButtonText;
        }

        // Check response format
        if (response && response.isSuccess !== undefined) {
            // Standard API response format
            if (response.isSuccess) {
                this.showNotification('✅ ' + (response.message || 'Lưu thành công!'), 'success');

                // Trigger success event
                form.dispatchEvent(new CustomEvent('formSubmitSuccess', {
                    detail: { response, formData: this.getFormData(form) }
                }));

                // Auto close modal nếu có
                this.autoCloseModal(form);

                // Auto refresh grid nếu có
                this.autoRefreshGrid();
            } else {
                this.showNotification('❌ ' + (response.message || 'Có lỗi xảy ra!'), 'error');
            }
        } else {
            // Generic success
            this.showNotification('✅ Lưu thành công!', 'success');
            form.dispatchEvent(new CustomEvent('formSubmitSuccess', {
                detail: { response, formData: this.getFormData(form) }
            }));
            this.autoCloseModal(form);
            this.autoRefreshGrid();
        }
    }

    /**
     * Handle submit error
     * @param {Object} xhr - XMLHttpRequest object
     * @param {string} error - Error message
     * @param {HTMLElement} form - Form element
     * @param {HTMLElement} button - Submit button
     * @param {string} originalButtonText - Original button text
     */
    handleSubmitError(xhr, error, form, button, originalButtonText) {
        // Restore button state
        if (button) {
            button.disabled = false;
            button.innerHTML = originalButtonText;
        }

        let errorMessage = 'Có lỗi xảy ra khi lưu dữ liệu!';

        if (xhr && xhr.responseJSON && xhr.responseJSON.message) {
            errorMessage = xhr.responseJSON.message;
        } else if (xhr && xhr.responseText) {
            try {
                const response = JSON.parse(xhr.responseText);
                errorMessage = response.message || errorMessage;
            } catch (e) {
                // Keep default message
            }
        }

        this.showNotification('❌ ' + errorMessage, 'error');

        // Trigger error event
        form.dispatchEvent(new CustomEvent('formSubmitError', {
            detail: { error, xhr, formData: this.getFormData(form) }
        }));
    }

    /**
     * Fetch API fallback for submit
     * @param {string} url - Submit URL
     * @param {string} method - HTTP method
     * @param {Object} data - Form data
     * @param {HTMLElement} form - Form element
     * @param {HTMLElement} button - Submit button
     * @param {string} originalButtonText - Original button text
     */
    fetchSubmit(url, method, data, form, button, originalButtonText) {
        const options = {
            method: method.toUpperCase(),
            headers: {
                'Content-Type': 'application/json'
            }
        };

        if (method.toUpperCase() !== 'GET') {
            options.body = JSON.stringify(data);
        } else {
            const params = new URLSearchParams(data);
            url += '?' + params.toString();
        }

        fetch(url, options)
            .then(response => response.json())
            .then(data => this.handleSubmitSuccess(data, form, button, originalButtonText))
            .catch(error => this.handleSubmitError(null, error.message, form, button, originalButtonText));
    }

    /**
     * Auto close modal after successful submit
     * @param {HTMLElement} form - Form element
     */
    autoCloseModal(form) {
        const modal = form.closest('.modal');
        if (modal && typeof $ !== 'undefined') {
            $(modal).modal('hide');
        }
    }

    /**
     * Auto refresh data grid after successful submit
     */
    autoRefreshGrid() {
        // Refresh grid if available
        if (window.dataGridInstance) {
            // Try to find and refresh any active grid
            const $grids = $('[data-component="data-grid"]');
            $grids.each((index, grid) => {
                const gridId = grid.id;
                if (gridId) {
                    const config = window.dataGridInstance.getGrid(gridId);
                    if (config) {
                        window.dataGridInstance.refreshData(config);
                    }
                }
            });
        }
    }

    /**
     * Show notification message
     * @param {string} message - Message to show
     * @param {string} type - Notification type (success, error, info, warning)
     */
    showNotification(message, type = 'info') {
        // Try SweetAlert first
        if (typeof Swal !== 'undefined') {
            const icon = type === 'error' ? 'error' : type === 'success' ? 'success' : 'info';
            Swal.fire({
                title: type === 'error' ? 'Lỗi!' : type === 'success' ? 'Thành công!' : 'Thông báo',
                text: message,
                icon: icon,
                timer: 3000,
                showConfirmButton: false
            });
            return;
        }

        // Fallback to custom notification
        const alertClass = type === 'error' ? 'alert-danger' :
                          type === 'success' ? 'alert-success' :
                          type === 'warning' ? 'alert-warning' : 'alert-info';

        const notification = document.createElement('div');
        notification.className = `alert ${alertClass} alert-dismissible fade show position-fixed`;
        notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 350px; max-width: 500px;';
        notification.innerHTML = `
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            <div class="d-flex align-items-center">
                ${message}
            </div>
        `;

        document.body.appendChild(notification);

        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, 4000);
    }
}

// Khởi tạo global instance
window.formControlBinder = new FormControlBinder();

// Auto-init khi DOM ready using jQuery
$(document).ready(function() {
    window.formControlBinder.init();
});

// Export cho module systems
if (typeof module !== 'undefined' && module.exports) {
    module.exports = FormControlBinder;
}
