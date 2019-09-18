-- Taken from DCS to ensure proper serialization - Jabbers

local function basicSerialize(o)
    if type(o) == 'number' then
        return o
    elseif type(o) == 'boolean' then
        return tostring(o)
    elseif type(o) == 'string' then
        return string.format('%q', o)
    else
        return 'nil'
    end
end

local function getSortedPairs(tableValue)
    local result = {}

    for key, value in pairs(tableValue) do
        table.insert(result, {key = key, value = value})
    end

    local sortFunction = function(pair1, pair2)
        return pair1.key < pair2.key
    end

    table.sort(result, sortFunction)

    return result
end

local function serialize_failure_message(value, name)
    if name ~= nil then
        return 'nil,--Cannot serialize a ' .. type(value) .. ' :' .. name .. '\n'
    else
        return 'nil,--Cannot serialize a ' .. type(value) .. '\n'
    end
end

Serializer = {}

local Serializer_mt = {__index = Serializer}

function Serializer:new(fout)
    local s = setmetatable({}, Serializer_mt)
    s.fout = fout
    return s
end

function Serializer:failed_to_serialize(value, name)
    self.fout:write(serialize_failure_message(self, value, name))
end

function Serializer:serialize(name, value, level)
    local levelOffset = '\t'

    if level == nil then
        level = ''
    end

    self.fout:write(level, name, ' = ')

    local valueType = type(value)

    if valueType == 'number' or valueType == 'string' or valueType == 'boolean' then
        if level == '' then
			self.fout:write(basicSerialize(value), '\n')
		else
			self.fout:write(basicSerialize(value), ',\n')
		end
    elseif valueType == 'table' then
        self.fout:write('{\n')

        local sortedPairs = getSortedPairs(value)

        for i, pair in pairs(sortedPairs) do
            local k = pair.key
            local key

            if type(k) == 'number' then
                key = string.format('[%s]', k)
            else
                key = string.format('[%q]', k)
            end

            self:serialize(key, pair.value, level .. levelOffset)
        end

        if level == '' then
            self.fout:write(level .. '}\n')
        else
            self.fout:write(level .. '},\n')
        end
    else
        self:failed_to_serialize(value, name)
    end
end
