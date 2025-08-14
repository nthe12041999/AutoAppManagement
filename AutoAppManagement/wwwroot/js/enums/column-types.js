/**
 * Column Types Enum
 * Defines all available column types for DataGrid
 */
const ColumnTypes = {
    // Basic data types
    TEXT: 'text',
    NUMBER: 'number', 
    MONEY: 'money',
    DATE: 'date',
    DATETIME: 'datetime',
    TIME: 'time',
    ENUM: 'enum',
    BOOL: 'bool',
    
    // Special types
    ACTIONS: 'actions'
};

/**
 * Column Type Configuration
 * Defines default settings and validation for each type
 */
const ColumnTypeConfig = {
    [ColumnTypes.TEXT]: {
        sortable: true,
        searchable: true,
        defaultWidth: 'auto',
        format: null
    },
    
    [ColumnTypes.NUMBER]: {
        sortable: true,
        searchable: true,
        defaultWidth: '100px',
        format: {
            decimal: 0,
            thousandSeparator: ',',
            decimalSeparator: '.'
        }
    },
    
    [ColumnTypes.MONEY]: {
        sortable: true,
        searchable: false,
        defaultWidth: '120px',
        format: {
            currency: 'VND',
            showSymbol: true,
            decimal: 0,
            thousandSeparator: ',',
            decimalSeparator: '.'
        }
    },
    
    [ColumnTypes.DATE]: {
        sortable: true,
        searchable: false,
        defaultWidth: '120px',
        format: {
            locale: 'vi-VN',
            style: 'dd/MM/yyyy'
        }
    },
    
    [ColumnTypes.DATETIME]: {
        sortable: true,
        searchable: false,
        defaultWidth: '160px',
        format: {
            locale: 'vi-VN',
            style: 'dd/MM/yyyy HH:mm',
            showSeconds: false,
            relative: false // true để hiển thị "2 giờ trước"
        }
    },
    
    [ColumnTypes.TIME]: {
        sortable: true,
        searchable: false,
        defaultWidth: '80px',
        format: {
            style: 'HH:mm',
            showSeconds: false
        }
    },
    
    [ColumnTypes.ENUM]: {
        sortable: true,
        searchable: true,
        defaultWidth: '120px',
        format: {
            badge: true,
            badgeColors: {
                'active': 'success',
                'inactive': 'secondary',
                'pending': 'warning',
                'suspended': 'danger',
                'hoạt động': 'success',
                'không hoạt động': 'secondary',
                'chờ duyệt': 'warning',
                'tạm ngưng': 'danger'
            },
            showIcon: false
        }
    },
    
    [ColumnTypes.BOOL]: {
        sortable: true,
        searchable: false,
        defaultWidth: '80px',
        format: {
            trueText: 'Có',
            falseText: 'Không',
            trueClass: 'text-success',
            falseClass: 'text-danger',
            showIcon: true,
            trueIcon: 'bi-check-circle',
            falseIcon: 'bi-x-circle'
        }
    },
    
    [ColumnTypes.ACTIONS]: {
        sortable: false,
        searchable: false,
        defaultWidth: '120px',
        format: null
    }
};

/**
 * Get column type configuration
 * @param {string} type - Column type
 * @returns {object} Type configuration
 */
function getColumnTypeConfig(type) {
    return ColumnTypeConfig[type] || ColumnTypeConfig[ColumnTypes.TEXT];
}

/**
 * Validate column type
 * @param {string} type - Column type to validate
 * @returns {boolean} True if valid
 */
function isValidColumnType(type) {
    return Object.values(ColumnTypes).includes(type);
}

/**
 * Get all available column types
 * @returns {Array} Array of column types
 */
function getAllColumnTypes() {
    return Object.values(ColumnTypes);
}

// Export for browser usage
if (typeof window !== 'undefined') {
    window.ColumnTypes = ColumnTypes;
    window.ColumnTypeConfig = ColumnTypeConfig;
    window.getColumnTypeConfig = getColumnTypeConfig;
    window.isValidColumnType = isValidColumnType;
    window.getAllColumnTypes = getAllColumnTypes;
}

// Export for Node.js usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        ColumnTypes,
        ColumnTypeConfig,
        getColumnTypeConfig,
        isValidColumnType,
        getAllColumnTypes
    };
}
