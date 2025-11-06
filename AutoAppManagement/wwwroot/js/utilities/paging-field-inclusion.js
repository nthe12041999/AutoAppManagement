/**
 * Utility class để tạo PagingRequest - SIMPLIFIED
 */
class PagingRequestBuilder {
    constructor() {
        this.request = {
            PageIndex: 1,
            PageSize: 10,
            Filter: "",
            Sort: "Id",
            RequestedColumns: []     // CHỈ CẦN CÁI NÀY THÔI!
        };
    }

    /**
     * Set pagination parameters
     */
    setPaging(pageIndex, pageSize) {
        this.request.PageIndex = pageIndex;
        this.request.PageSize = pageSize;
        return this;
    }

    /**
     * Set filter string
     */
    setFilter(filter) {
        this.request.Filter = filter || "";
        return this;
    }

    /**
     * Set sort field
     */
    setSort(sortField) {
        this.request.Sort = sortField || "Id";
        return this;
    }

    /**
     * Set requested columns từ grid configuration (FE gửi xuống BE)
     */
    setRequestedColumns(columns) {
        this.request.RequestedColumns = columns || [];
        return this;
    }

    /**
     * Auto extract columns từ grid config object
     */
    extractColumnsFromGridConfig(gridConfig) {
        if (gridConfig && gridConfig.columns) {
            const columnFields = gridConfig.columns.map(col => col.field).filter(field => field);
            this.request.RequestedColumns = columnFields;
        }
        return this;
    }

    /**
     * Build the final request object
     */
    build() {
        return this.request;
    }
}

/**
 * Example usage trong grid - SIMPLIFIED
 */
class AccountGridSimple {
    constructor() {
        this.apiUrl = '/Account/GetPagingWithFields';
    }

    /**
     * Load data với columns từ grid config
     */
    async loadData(pageIndex = 1, pageSize = 10, filter = "", gridConfig = null) {
        try {
            // Tạo request đơn giản
            const request = new PagingRequestBuilder()
                .setPaging(pageIndex, pageSize)
                .setFilter(filter)
                .extractColumnsFromGridConfig(gridConfig || window.currentGridConfig)
                .build();

            console.log('Simple request:', request);

            const response = await fetch(this.apiUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(request)
            });

            const result = await response.json();
            return result.IsSuccess ? result.Data : null;
        } catch (error) {
            console.error('Error loading data:', error);
            throw error;
        }
    }

    /**
     * Load data với explicit columns
     */
    async loadDataWithColumns(pageIndex, pageSize, filter, columns) {
        const request = new PagingRequestBuilder()
            .setPaging(pageIndex, pageSize)
            .setFilter(filter)
            .setRequestedColumns(columns)  // Chỉ set columns thôi!
            .build();

        console.log('Request with explicit columns:', request);

        const response = await fetch(this.apiUrl, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(request)
        });

        const result = await response.json();
        return result.IsSuccess ? result.Data : null;
    }
}

// Example sử dụng đơn giản
/*
const accountGrid = new AccountGridSimple();

// Load với grid config
accountGrid.loadData(1, 10, "", gridConfig).then(data => {
    console.log('Data:', data);
});

// Load với explicit columns
const columns = ['Name', 'Email', 'LicenseName', 'StatusName'];
accountGrid.loadDataWithColumns(1, 10, "", columns).then(data => {
    console.log('Data with specific columns:', data);
});
*/