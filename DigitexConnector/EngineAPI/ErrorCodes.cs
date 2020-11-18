namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Error codes.
    /// </summary>
    public enum ErrorCodes
    {
        ERROR_NONE,

        ERROR_UNIDENTIFIED,                 // 1
        ERROR_NOT_IMPLEMENTED,              // 2
        ERROR_UUID_ALREADY_EXISTS,          // 3
        ERROR_CURRENCY_SIZE_NOT_SUPPORTED,  // 4
        ERROR_SYSTEM,                       // 5
        ERROR_ORDER_TYPE_NOT_SUPPORTED,     // 6
        ERROR_ORDER_SIDE_NOT_SUPPORTED,     // 7
        ERROR_ORDER_DURATION_NOT_SUPPORTED, // 8
        ERROR_ALLOCATION_FAILED,            // 9
        ERROR_UUID_DOES_NOT_EXIST,          // 10
        ERROR_INTERNAL,                     // 11
        ERROR_FILESYSTEM,                   // 12
        ERROR_INVALID_MARKET_ID,            // 13
        ERROR_INVALID_TRADER_ID,            // 14
        ERROR_INVALID_ORDER_TYPE,           // 15
        ERROR_INVALID_ORDER_SIDE,           // 16
        ERROR_INVALID_ORDER_DURATION,       // 17
        ERROR_INVALID_LEVERAGE,             // 18
        ERROR_INVALID_PRICE,                // 19
        ERROR_INVALID_QUANTITY,             // 20
        ERROR_PERMISSON_DENIED,             // 21
        ERROR_NO_MARKET_PRICE,              // 22
        ERROR_PRICE_DOES_NOT_MATCH,         // 23
        ERROR_SIDE_DOES_NOT_MATCH,          // 24
        ERROR_TYPE_DOES_NOT_MATCH,          // 25
        ERROR_INVALID_CURRENCY_ID,          // 26
        ERROR_NOT_ENOUGH_BALANCE,           // 27
        ERROR_OVERFLOW,                     // 28
        ERROR_INVALID_SCALE,                // 29
        ERROR_LOSS_OF_PRECISION,            // 30
        ERROR_INVALID_STOP_LOSS_PRICE,      // 31
        ERROR_INVALID_TAKE_PROFIT_PRICE,    // 32
        ERROR_UNIDENTIFIED_AMQP,            // 33
        ERROR_INVALID_CONTRACT_ID,          // 34
        ERROR_RATE_LIMIT_EXCEEDED,          // 35
        ERROR_NO_CONTRACTS,                 // 36
        ERROR_NO_OPPOSING_ORDERS,           // 37
        ERROR_FILE_NOT_FOUND,               // 38
        ERROR_RESTRICTED_TRADER_ID,         // 39
        ERROR_PRICE_WORSE_LIQUIDATION,      // 40
        ERROR_INVALID_BALANCE,              // 41
        ERROR_INVALID_POSITION,             // 42
        ERROR_INVALID_NEW_TRADER_ID,        // 43
        ERROR_UNSUPPORTED_VERSION,          // 44
        ERROR_TOURNAMENT_IN_PROGRESS,       // 45
        ERROR_NOT_MAIN_TRADER_ID,           // 46
        ERROR_INVALID_INITIAL_BALANCE,      // 47
        ERROR_INVALID_MAX_POSITION,         // 48
        ERROR_INVALID_START_TIMESTAMP,      // 49
        ERROR_INVALID_DURATION,             // 50
        ERROR_RUN_FINISHED,                 // 51
        ERROR_TRANSFER_DISABLED,            // 52
        ERROR_MAX_QUANTITY_EXCEEDED,        // 53
        ERROR_PNL_TOO_NEGATIVE,             // 54
        ERROR_ORDER_WOULD_BE_INVALID,       // 55 - there is an order that after leverage change would appear below/above the liquidation point
        ERROR_INVALID_VALUE_CODE,           // 56
        ERROR_INVALID_VALUE,                // 57
        ERROR_TRADING_SUSPENDED,            // 58
        ERROR_WITHDRAWAL_SUSPENDED,         // 59
        ERROR_DEPOSITS_SUSPENDED,           // 60
        ERROR_SPOT_PRICE_EXCURSION,         // 61
        ERROR_INVALID_SETTLEMENT_PRICE,     // 62
        ERROR_CANNOT_BE_FILLED,             // 63 - "fill or kill" or "immediate or cancel" order cannot be filled immediately
        ERROR_INVALID_MESSAGE,              // 64
        ERROR_TOO_MANY_DELAYED_ACTIONS,     // 65
        ERROR_OPERATION_FAILED,             // 66
        ERROR_RISK_CHECK_FAILED,            // 67 - error for failing exchange risk managment policy
        ERROR_TOO_MANY_ORDERS,              // 68
    };
}
