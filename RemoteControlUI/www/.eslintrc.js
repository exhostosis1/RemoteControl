module.exports = {
    env: {
        browser: true,
        es6: true,
    },
    extends: [
        'airbnb-base',
    ],
    globals: {
        Atomics: 'readonly',
        SharedArrayBuffer: 'readonly',
    },
    parserOptions: {
        ecmaVersion: 11,
        sourceType: 'module',
    },
    rules: {
        indent: ['error', 4],
        'linebreak-style': ['error', 'windows'],
        quotes: ['warn', 'single'],
        'import/extensions': ['error', 'always'],
        'arrow-parens': ['error', 'as-needed'],
        'max-len': ['warn', 256],
    },
};
