function serializeToFile(path, name, t, mode, brackets)
	if t then
		local file, err = io.open(path, mode or 'w')

		brackets = brackets or true 

		if file then
			local s = Serializer:new(file)
			s:serialize(name, t, '', brackets)
			file:close()
		else
			print(err)
		end
	end
end