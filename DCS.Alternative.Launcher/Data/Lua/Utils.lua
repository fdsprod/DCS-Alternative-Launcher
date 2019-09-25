
table = table or {}

function table.merge(a, b) 
  for k,v in pairs(b) do
        if type(v) == "table" and type(a[k] or false) == "table" then
          merge(a[k], v)
        else
          a[k] = v
        end
  end  
  return a
end
